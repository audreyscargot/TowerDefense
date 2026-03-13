using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")] 
    public int mapSize = 10;
    public Tilemap groundTilemap;

    [Header("Category 1: Base Tiles (Ground)")]
    public TileBase[] baseTiles;

    [Header("Category 2: Resource Objects")]
    public GameObject[] resourcePrefabs;

    [Range(0f, 1f)] public float resourceSpawnChance = 0.15f;

    private bool[,] obstacleGrid;
    
    private List<Vector2Int> obstacleCoordinates = new List<Vector2Int>();

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
                    float pX = (x + seedX);
                    float pY = (y + seedY);
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
                    obstacleGrid[arrayX, arrayY] = false;
                    continue;
                }

                if (resourcePrefabs != null && resourcePrefabs.Length > 0 && Random.value < resourceSpawnChance)
                {
                    GameObject randomResource = resourcePrefabs[Random.Range(0, resourcePrefabs.Length)];
                    Vector3 spawnPos = groundTilemap.GetCellCenterWorld(gridPosition);
                    Instantiate(randomResource, spawnPos, Quaternion.identity, transform);
                    
                    obstacleGrid[arrayX, arrayY] = true;
                    obstacleCoordinates.Add(new Vector2Int(x, y));
                }
                else
                {
                    obstacleGrid[arrayX, arrayY] = false;
                }
            }
        }
        
        
    }
    
    public bool[,] GetObstacleGrid()
    {
        return obstacleGrid;
    }


    public List<Vector2Int> GetObstacleCoordinates()
    {
        return obstacleCoordinates;
    }
    
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
