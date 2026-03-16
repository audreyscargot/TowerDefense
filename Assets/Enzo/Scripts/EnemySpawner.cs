using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public int baseEnemiesPerNight = 3;
    public float timeBetweenSpawns = 1.0f;
    public int spawnPointCount = 3;

    private GameManager gameManager;
    private List<GameObject> activeSpawnPoints = new List<GameObject>();
    private int enemiesAlive = 0;
    private int enemiesToSpawnThisNight = 0;
    private int enemiesSpawnedSoFar = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        GameManager.OnDayStarted += PrepareForNewDay;
        GameManager.OnNightStarted += StartNightWave;
    }

    private void OnDestroy()
    {
        GameManager.OnDayStarted -= PrepareForNewDay;
        GameManager.OnNightStarted -= StartNightWave;
    }

    private void PrepareForNewDay()
    {
        // Stop any wave coroutine BEFORE destroying spawn points
        StopAllCoroutines();

        foreach (GameObject sp in activeSpawnPoints)
        {
            if (sp != null) Destroy(sp);
        }
        activeSpawnPoints.Clear();

        if (MapGenerator.Instance == null)
        {
            Debug.LogWarning("EnemySpawner: No MapGenerator instance found.");
            return;
        }

        if (MapGenerator.Instance.aiSpawnPointPrefab == null)
        {
            Debug.LogWarning("EnemySpawner: aiSpawnPointPrefab is not assigned on MapGenerator.");
            return;
        }

        for (int i = 0; i < spawnPointCount; i++)
        {
            Vector3 edgePos = MapGenerator.Instance.GetRandomEdgeWorldPosition();
            GameObject sp = Instantiate(MapGenerator.Instance.aiSpawnPointPrefab, edgePos, Quaternion.identity);
            activeSpawnPoints.Add(sp);
        }

        Debug.Log($"Prepared {activeSpawnPoints.Count} spawn points for the new day.");
    }

    private void StartNightWave()
    {
        if (activeSpawnPoints.Count == 0) return;

        int currentDay = gameManager != null ? (int)gameManager.Days : 1;
        enemiesToSpawnThisNight = baseEnemiesPerNight + (currentDay * 2);
        enemiesSpawnedSoFar = 0;
        enemiesAlive = 0;

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (enemiesSpawnedSoFar < enemiesToSpawnThisNight)
        {
            activeSpawnPoints.RemoveAll(sp => sp == null);

            if (activeSpawnPoints.Count == 0)
            {
                Debug.LogWarning("All spawn points were destroyed during the wave.");
                yield break;
            }

            Transform randomPoint = activeSpawnPoints[Random.Range(0, activeSpawnPoints.Count)].transform;
            Instantiate(enemyPrefab, randomPoint.position, Quaternion.identity);
            enemiesAlive++;
            enemiesSpawnedSoFar++;

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    public void EnemyDied()
    {
        enemiesAlive--;

        if (enemiesSpawnedSoFar >= enemiesToSpawnThisNight && enemiesAlive <= 0)
        {
            Debug.Log("Wave Defeated! Starting next day...");
            gameManager.EndNight();
        }
    }
}
