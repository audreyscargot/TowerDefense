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

    [Header("Visuals")]
    public GameObject spawnIndicatorPrefab;

    private GameManager gameManager;
    private List<Vector3> spawnPositions = new List<Vector3>();
    private List<GameObject> spawnIndicators = new List<GameObject>();
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
        // Day 1 indicators are spawned by MapGenerator.GenerateFullMap()
        // so no manual call needed here
    }

    private void OnDestroy()
    {
        GameManager.OnDayStarted -= PrepareForNewDay;
        GameManager.OnNightStarted -= StartNightWave;
    }

    public void PrepareForNewDay()
    {
        StopAllCoroutines();

        // Destroy previous indicators
        foreach (GameObject indicator in spawnIndicators)
            if (indicator != null) Destroy(indicator);
        spawnIndicators.Clear();
        spawnPositions.Clear();

        if (MapGenerator.Instance == null)
        {
            Debug.LogWarning("EnemySpawner: No MapGenerator instance found.");
            return;
        }

        for (int i = 0; i < spawnPointCount; i++)
        {
            Vector3 edgePos = MapGenerator.Instance.GetRandomEdgeWorldPosition();
            spawnPositions.Add(edgePos);

            if (spawnIndicatorPrefab != null)
                spawnIndicators.Add(Instantiate(spawnIndicatorPrefab, edgePos, Quaternion.identity));
        }

        Debug.Log($"Prepared {spawnPositions.Count} spawn positions.");
    }

    private void StartNightWave()
    {
        // Indicators stay visible during the night
        if (spawnPositions.Count == 0) return;
        StartCoroutine(StartWaveDelayed());
    }

    private IEnumerator StartWaveDelayed()
    {
        yield return new WaitForSeconds(0.5f); // give NavMesh time to finish baking
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
            Vector3 randomPos = spawnPositions[Random.Range(0, spawnPositions.Count)];
            Instantiate(enemyPrefab, randomPos, Quaternion.identity);
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
