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
        GameObject[] allSpawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        activeSpawnPoints.Clear();

        if (allSpawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points found yet. The map might still be generating.");
            return;
        }

        int pointsToActivate = Random.Range(1, Mathf.Min(4, allSpawnPoints.Length + 1));
        
        List<GameObject> shuffledPoints = new List<GameObject>(allSpawnPoints);
        for (int i = 0; i < shuffledPoints.Count; i++)
        {
            GameObject temp = shuffledPoints[i];
            int randomIndex = Random.Range(i, shuffledPoints.Count);
            shuffledPoints[i] = shuffledPoints[randomIndex];
            shuffledPoints[randomIndex] = temp;
        }

        foreach (var sp in allSpawnPoints)
        {
            sp.GetComponent<SpriteRenderer>().enabled = false;
        }

        for (int i = 0; i < pointsToActivate; i++)
        {
            activeSpawnPoints.Add(shuffledPoints[i]);
            shuffledPoints[i].GetComponent<SpriteRenderer>().enabled = true; 
        }
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