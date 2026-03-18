using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public int baseEnemiesPerNight = 1;
    public float timeBetweenSpawns = 1.0f;
    public int spawnPointCount = 1;

    [Header("Spawner Clearance")]
    public float resourceClearRadius = 2f; // world units cleared around each spawn point

    [Header("Visuals")]
    public GameObject spawnIndicatorPrefab;

    private GameManager gameManager;
    private List<Vector3> spawnPositions = new List<Vector3>();
    private List<GameObject> spawnIndicators = new List<GameObject>();
    private int enemiesAlive = 0;
    private int enemiesToSpawnThisNight = 0;
    private int enemiesSpawnedSoFar = 0;

    private bool skipFirstDayEvent = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        GameManager.OnDayStarted += OnDayStartedHandler;
        GameManager.OnNightStarted += StartNightWave;
    }

    private void OnDestroy()
    {
        GameManager.OnDayStarted -= OnDayStartedHandler;
        GameManager.OnNightStarted -= StartNightWave;
    }

    private void OnDayStartedHandler()
    {
        if (skipFirstDayEvent) { skipFirstDayEvent = false; return; }
        PrepareForNewDay();
    }

    public void PrepareForNewDay()
    {
        StopAllCoroutines();

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

            // Clear resources around this spawn point
            MapGenerator.Instance.ClearResourcesNearPosition(edgePos, resourceClearRadius);

            if (spawnIndicatorPrefab != null)
                spawnIndicators.Add(Instantiate(spawnIndicatorPrefab, edgePos, Quaternion.identity));
        }

        Debug.Log($"Prepared {spawnPositions.Count} spawn positions.");
    }

    private void StartNightWave()
    {
        if (spawnPositions.Count == 0) return;
        StartCoroutine(StartWaveDelayed());
    }

    private IEnumerator StartWaveDelayed()
    {
        yield return new WaitForSeconds(0.5f);
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

    public List<Vector3> GetSpawnPositions() => spawnPositions;
}
