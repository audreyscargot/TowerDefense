using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")] public int mapSize = 10;
    public Tilemap groundTilemap;
    public float biomeScale = 0.1f;

    [Header("Category 1: Base Tiles (Ground)")]
    public TileBase[] baseTiles;

    [Header("Category 2: Resource Objects")]
    public GameObject[] resourcePrefabs;

    [Range(0f, 1f)] public float resourceSpawnChance = 0.15f;

    void Start()
    {
        GenerateFullMap();
    }

    public void GenerateFullMap()
    {
        int halfSize = mapSize / 2;

        float seedX = Random.Range(0f, 10000f);
        float seedY = Random.Range(0f, 10000f);

        for (int x = -halfSize; x < halfSize; x++)
        {
            for (int y = -halfSize; y < halfSize; y++)
            {
                Vector3Int gridPosition = new Vector3Int(x, y, 0);

                if (baseTiles != null && baseTiles.Length > 0)
                {
                    float pX = (x + seedX) * biomeScale;
                    float pY = (y + seedY) * biomeScale;
                    float noiseValue = Mathf.PerlinNoise(pX, pY);

                    int tileIndex = 0;
                    if (baseTiles.Length > 1)
                    {
                        tileIndex = Mathf.Clamp(Mathf.FloorToInt(noiseValue * baseTiles.Length), 0,
                            baseTiles.Length - 1);
                    }

                    groundTilemap.SetTile(gridPosition, baseTiles[tileIndex]);
                }

                if (x == 0 && y == 0)
                {
                    continue;
                }

                if (resourcePrefabs != null && resourcePrefabs.Length > 0 && Random.value < resourceSpawnChance)
                {
                    GameObject randomResource = resourcePrefabs[Random.Range(0, resourcePrefabs.Length)];
                    Vector3 spawnPos = groundTilemap.GetCellCenterWorld(gridPosition);
                    Instantiate(randomResource, spawnPos, Quaternion.identity, transform);
                }
            }
        }
    }
}