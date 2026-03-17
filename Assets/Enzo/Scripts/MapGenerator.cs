using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    [Header("Map Settings")]
    public int mapSize = 50;
    public Tilemap groundTilemap;

    [Header("Safe Zone")]
    public float SafeZoneWorldRadius = 5f;
    public Vector2 MapCenterWorld => groundTilemap.GetCellCenterWorld(Vector3Int.zero);

    [Header("Category 1: Base Tiles (Ground)")]
    public TileBase[] baseTiles;

    [Header("Category 2: Resource Objects")]
    public GameObject[] resourcePrefabs;
    [Range(0f, 1f)] public float resourceSpawnChance = 0.15f;

    [Header("Category 3: Core Structures")]
    public GameObject basePrefab;

    [Header("NavMesh")]
    public NavMesh navMeshBaker;

    private bool[,] obstacleGrid;
    private List<Vector2Int> obstacleCoordinates = new List<Vector2Int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        GenerateFullMap();
    }

    public void GenerateFullMap()
    {
        int halfSize = mapSize / 2;

        obstacleGrid = new bool[mapSize, mapSize];
        obstacleCoordinates.Clear();

        float seedX = Random.Range(0f, 10000f);
        float seedY = Random.Range(0f, 10000f);

        for (int x = -halfSize; x < halfSize; x++)
        {
            for (int y = -halfSize; y < halfSize; y++)
            {
                Vector3Int gridPosition = new Vector3Int(x, y, 0);
                int arrayX = x + halfSize;
                int arrayY = y + halfSize;

                if (baseTiles != null && baseTiles.Length > 0)
                {
                    float noiseValue = Mathf.PerlinNoise(x + seedX, y + seedY);
                    int tileIndex = Mathf.Clamp(Mathf.FloorToInt(noiseValue * baseTiles.Length), 0, baseTiles.Length - 1);
                    groundTilemap.SetTile(gridPosition, baseTiles[tileIndex]);
                }

                if (x == 0 && y == 0)
                {
                    obstacleGrid[arrayX, arrayY] = false;
                    continue;
                }

                if (resourcePrefabs != null && resourcePrefabs.Length > 0 && Random.value < resourceSpawnChance)
                {
                    Vector3 spawnPos = groundTilemap.GetCellCenterWorld(gridPosition);
                    Instantiate(resourcePrefabs[Random.Range(0, resourcePrefabs.Length)], spawnPos, Quaternion.identity, transform);
                    obstacleGrid[arrayX, arrayY] = true;
                    obstacleCoordinates.Add(new Vector2Int(x, y));
                }
                else
                {
                    obstacleGrid[arrayX, arrayY] = false;
                }
            }
        }

        // Place base at map center
        if (basePrefab != null)
        {
            Vector3 centerWorld = groundTilemap.GetCellCenterWorld(Vector3Int.zero);
            GameObject baseObj = Instantiate(basePrefab, centerWorld, Quaternion.identity, transform);
            baseObj.name = "Base";
        }

        // Bake NavMesh AFTER map is fully generated
        if (navMeshBaker != null)
            navMeshBaker.BakeNavMesh();
        else
            Debug.LogWarning("MapGenerator: navMeshBaker is not assigned!");

        // Spawn first day indicators — same logic as every subsequent day
        if (EnemySpawner.Instance != null)
            EnemySpawner.Instance.PrepareForNewDay();
    }

    public Vector3 GetRandomEdgeWorldPosition()
    {
        int halfSize = mapSize / 2;
        int x, y;

        switch (Random.Range(0, 4))
        {
            case 0:  x = -halfSize;    y = Random.Range(-halfSize, halfSize); break; // left
            case 1:  x = halfSize - 1; y = Random.Range(-halfSize, halfSize); break; // right
            case 2:  x = Random.Range(-halfSize, halfSize); y = -halfSize;    break; // bottom
            default: x = Random.Range(-halfSize, halfSize); y = halfSize - 1; break; // top
        }

        return groundTilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
    }

    public bool[,] GetObstacleGrid() => obstacleGrid;
    public List<Vector2Int> GetObstacleCoordinates() => obstacleCoordinates;

    public void RemoveObstacleAt(Vector2Int worldGridPosition)
    {
        int arrayX = worldGridPosition.x + (mapSize / 2);
        int arrayY = worldGridPosition.y + (mapSize / 2);

        if (arrayX >= 0 && arrayX < mapSize && arrayY >= 0 && arrayY < mapSize)
        {
            obstacleGrid[arrayX, arrayY] = false;
            obstacleCoordinates.Remove(worldGridPosition);
        }
    }
}
