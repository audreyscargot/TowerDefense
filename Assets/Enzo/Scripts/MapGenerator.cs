using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class ResourcePrefabEntry
{
    public GameObject prefab;
    [Range(0f, 1f)] public float spawnThreshold = 0.65f;
    [Range(0f, 1f)] public float noiseScale = 0.15f;
    [Range(0f, 1f)] public float respawnChancePerDay = 0.3f;
}

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    [Header("Map Settings")]
    public int mapSize = 50;
    public Tilemap groundTilemap;

    [Header("Safe Zone")]
    public float SafeZoneRadiusDividend = 5f;
    public float SafeZoneWorldRadius => mapSize / SafeZoneRadiusDividend;
    public Vector2 MapCenterWorld => groundTilemap.GetCellCenterWorld(Vector3Int.zero);

    [Header("Category 1: Base Tiles")]
    public TileBase[] baseTiles;

    [Header("Category 2: Resource Objects")]
    public ResourcePrefabEntry[] resourcePrefabs;

    [Header("Category 3: Core Structures")]
    public GameObject basePrefab;

    private bool[,] obstacleGrid;
    private List<Vector2Int> obstacleCoordinates = new List<Vector2Int>();

    private struct DestroyedNode
    {
        public Vector3 worldPosition;
        public int prefabIndex;
    }

    private List<DestroyedNode> destroyedNodes = new List<DestroyedNode>();
    private Vector2[] resourceNoiseSeeds;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()  { GameManager.OnDayStarted += RespawnResources; }
    private void OnDisable() { GameManager.OnDayStarted -= RespawnResources; }

    private void Start() { GenerateFullMap(); }

    public void GenerateFullMap()
    {
        int halfSize = mapSize / 2;
        obstacleGrid = new bool[mapSize, mapSize];
        obstacleCoordinates.Clear();
        destroyedNodes.Clear();

        resourceNoiseSeeds = new Vector2[resourcePrefabs.Length];
        for (int i = 0; i < resourcePrefabs.Length; i++)
            resourceNoiseSeeds[i] = new Vector2(Random.Range(0f, 10000f), Random.Range(0f, 10000f));

        float tileSeedX = Random.Range(0f, 10000f);
        float tileSeedY = Random.Range(0f, 10000f);

        for (int x = -halfSize; x < halfSize; x++)
        {
            for (int y = -halfSize; y < halfSize; y++)
            {
                Vector3Int gridPosition = new Vector3Int(x, y, 0);
                int arrayX = x + halfSize;
                int arrayY = y + halfSize;

                if (baseTiles != null && baseTiles.Length > 0)
                {
                    float noiseValue = Mathf.PerlinNoise(x + tileSeedX, y + tileSeedY);
                    int tileIndex = Mathf.Clamp(Mathf.FloorToInt(noiseValue * baseTiles.Length), 0, baseTiles.Length - 1);
                    groundTilemap.SetTile(gridPosition, baseTiles[tileIndex]);
                }

                if (x == 0 && y == 0) { obstacleGrid[arrayX, arrayY] = false; continue; }

                Vector3 worldPos = groundTilemap.GetCellCenterWorld(gridPosition);
                if (Vector2.Distance(worldPos, MapCenterWorld) <= SafeZoneWorldRadius)
                {
                    obstacleGrid[arrayX, arrayY] = false;
                    continue;
                }

                bool spawned = false;
                for (int i = 0; i < resourcePrefabs.Length; i++)
                {
                    ResourcePrefabEntry entry = resourcePrefabs[i];
                    float noise = Mathf.PerlinNoise(
                        x * entry.noiseScale + resourceNoiseSeeds[i].x,
                        y * entry.noiseScale + resourceNoiseSeeds[i].y
                    );

                    if (noise > entry.spawnThreshold)
                    {
                        SpawnResourceNode(i, worldPos);
                        obstacleGrid[arrayX, arrayY] = true;
                        obstacleCoordinates.Add(new Vector2Int(x, y));
                        spawned = true;
                        break;
                    }
                }

                if (!spawned) obstacleGrid[arrayX, arrayY] = false;
            }
        }

        if (basePrefab != null)
            Instantiate(basePrefab, groundTilemap.GetCellCenterWorld(Vector3Int.zero), Quaternion.identity, transform).name = "Base";

        AStarGrid astar = GetComponent<AStarGrid>();
        if (astar != null)
        {
            astar.gridWorldSize = new Vector2(mapSize * groundTilemap.cellSize.x, mapSize * groundTilemap.cellSize.y);
            astar.BuildGridFromObstacles(obstacleGrid, mapSize);
        }
        else Debug.LogWarning("MapGenerator: No AStarGrid component found!");

        if (EnemySpawner.Instance != null)
            EnemySpawner.Instance.PrepareForNewDay();
    }

    public void ClearResourcesNearPosition(Vector3 worldPos, float worldRadius)
    {
        ResourceNode[] allNodes = GetComponentsInChildren<ResourceNode>();
        foreach (ResourceNode node in allNodes)
        {
            if (Vector3.Distance(node.transform.position, worldPos) <= worldRadius)
            {
                AStarGrid.Instance?.SetNodeWalkable(node.transform.position, true);
                Destroy(node.gameObject);
            }
        }
    }

    private GameObject SpawnResourceNode(int prefabIndex, Vector3 worldPos)
    {
        if (resourcePrefabs == null || prefabIndex >= resourcePrefabs.Length) return null;

        GameObject obj = Instantiate(resourcePrefabs[prefabIndex].prefab, worldPos, Quaternion.identity, transform);
        ResourceNode node = obj.GetComponent<ResourceNode>();
        if (node != null) { node.prefabIndex = prefabIndex; node.spawnPosition = worldPos; }
        return obj;
    }

    public void OnResourceDestroyed(ResourceNode node)
    {
        destroyedNodes.Add(new DestroyedNode { worldPosition = node.spawnPosition, prefabIndex = node.prefabIndex });
        AStarGrid.Instance?.SetNodeWalkable(node.spawnPosition, true);
    }

    private void RespawnResources()
    {
        List<DestroyedNode> stillDead = new List<DestroyedNode>();
        foreach (DestroyedNode dead in destroyedNodes)
        {
            float chance = resourcePrefabs[dead.prefabIndex].respawnChancePerDay;
            if (Random.value < chance)
            {
                SpawnResourceNode(dead.prefabIndex, dead.worldPosition);
                AStarGrid.Instance?.SetNodeWalkable(dead.worldPosition, false);
            }
            else stillDead.Add(dead);
        }
        destroyedNodes = stillDead;
    }

    public Vector3 GetRandomEdgeWorldPosition()
    {
        int halfSize = mapSize / 2;
        int x, y;
        switch (Random.Range(0, 4))
        {
            case 0:  x = -halfSize;    y = Random.Range(-halfSize, halfSize); break;
            case 1:  x = halfSize - 1; y = Random.Range(-halfSize, halfSize); break;
            case 2:  x = Random.Range(-halfSize, halfSize); y = -halfSize;    break;
            default: x = Random.Range(-halfSize, halfSize); y = halfSize - 1; break;
        }
        return groundTilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
    }

    public bool[,] GetObstacleGrid() => obstacleGrid;
    public List<Vector2Int> GetObstacleCoordinates() => obstacleCoordinates;

    public void RemoveObstacleAt(Vector2Int worldGridPosition)
    {
        int arrayX = worldGridPosition.x + mapSize / 2;
        int arrayY = worldGridPosition.y + mapSize / 2;
        if (arrayX >= 0 && arrayX < mapSize && arrayY >= 0 && arrayY < mapSize)
        {
            obstacleGrid[arrayX, arrayY] = false;
            obstacleCoordinates.Remove(worldGridPosition);
        }
    }
}
