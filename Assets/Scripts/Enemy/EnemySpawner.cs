using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _meleePrefab;
    [SerializeField] private GameObject _rangePrefab;
    [SerializeField] private GameObject _suicidePrefab;

    [SerializeField] private float _minimumSpawnTime;
    [SerializeField] private float _maximumSpawnTime;

    [SerializeField] private Vector2 _spawnAreaMin;
    [SerializeField] private Vector2 _spawnAreaMax;

    [SerializeField] private int _meleeCount = 5;
    [SerializeField] private int _rangeCount = 3;
    [SerializeField] private int _suicideCount = 1;
    
    private int _currentMeleeCount = 0;
    private int _currentRangeCount = 0;
    private int _currentSuicideCount = 0;
    
    private const string MELEE_TAG = "MeleeEnemy";
    private const string RANGE_TAG = "RangeEnemy";
    private const string SUICIDE_TAG = "SuicideEnemy";
    
    private float _timeUntilSpawn;
    private List<GameObject> _activeEnemies = new List<GameObject>();
    [SerializeField] private float _minDistanceBetweenEnemies = 2f;
    [SerializeField] private float _minDistanceFromPlayer = 4f;
    
    private GameObject _player;

    void Awake()
    {
        SetTimeUntilSpawn();
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    
    void Start() => ValidateObjectPool();
    
    void Update()
    {
        _timeUntilSpawn -= Time.deltaTime;
        if (_timeUntilSpawn <= 0)
        {
            SpawnEnemy();
            SetTimeUntilSpawn();
        }
        _activeEnemies.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy);
    }

    private void ValidateObjectPool()
    {
        bool meelePoolExists = false, rangePoolExists = false, suicidePoolExists = false;
        foreach (ObjectPool.Pool pool in ObjectPool.Instance.pools)
        {
            if (pool.tag == MELEE_TAG) meelePoolExists = true;
            if (pool.tag == RANGE_TAG) rangePoolExists = true;
            if (pool.tag == SUICIDE_TAG) suicidePoolExists = true;
        }
    }

    private void SpawnEnemy()
    {
        string enemyTag = DetermineEnemyTypeToSpawn();
        Vector2 spawnPosition = FindValidSpawnPosition();
        if (spawnPosition == Vector2.zero) { SetTimeUntilSpawn(); return; }
        GameObject enemy = ObjectPool.Instance.SpawnFromPool(enemyTag, spawnPosition, Quaternion.identity);
        if (enemy == null || !enemy.activeInHierarchy) { SetTimeUntilSpawn(); return; }
        
        // Make sure enemy's layer collision is configured to allow passing through other enemies
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Set enemy's collision detection mode
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
        
        _activeEnemies.Add(enemy);
    }
    
    private Vector2 FindValidSpawnPosition()
    {
        const int MAX_ATTEMPTS = 30;
        for (int i = 0; i < MAX_ATTEMPTS; i++)
        {
            Vector2 randomPosition = new Vector2(Random.Range(_spawnAreaMin.x, _spawnAreaMax.x), Random.Range(_spawnAreaMin.y, _spawnAreaMax.y));
            
            // Check distance from player
            bool isTooCloseToPlayer = _player != null && Vector2.Distance(randomPosition, _player.transform.position) < _minDistanceFromPlayer;
            
            // Check distance from other enemies
            bool isTooCloseToEnemies = _activeEnemies.Exists(enemy => 
                enemy != null && 
                enemy.activeInHierarchy && 
                Vector2.Distance(randomPosition, enemy.transform.position) < _minDistanceBetweenEnemies);
            
            if (!isTooCloseToPlayer && !isTooCloseToEnemies)
                return randomPosition;
        }
        
        // If after MAX_ATTEMPTS we still can't find a valid position, try one more time with just player distance check
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomPosition = new Vector2(Random.Range(_spawnAreaMin.x, _spawnAreaMax.x), Random.Range(_spawnAreaMin.y, _spawnAreaMax.y));
            bool isTooCloseToPlayer = _player != null && Vector2.Distance(randomPosition, _player.transform.position) < _minDistanceFromPlayer;
            
            if (!isTooCloseToPlayer)
                return randomPosition;
        }
        
        return Vector2.zero;
    }

    private string DetermineEnemyTypeToSpawn()
    {
        if (_currentMeleeCount >= _meleeCount && _currentRangeCount >= _rangeCount && _currentSuicideCount >= _suicideCount)
        {
            _currentMeleeCount = 0;
            _currentRangeCount = 0;
            _currentSuicideCount = 0;
        }
        List<string> availableTypes = new List<string>();
        if (_currentMeleeCount < _meleeCount) availableTypes.Add(MELEE_TAG);
        if (_currentRangeCount < _rangeCount) availableTypes.Add(RANGE_TAG);
        if (_currentSuicideCount < _suicideCount) availableTypes.Add(SUICIDE_TAG);
        if (availableTypes.Count == 0) return DetermineEnemyTypeToSpawn();
        string selectedType = availableTypes[Random.Range(0, availableTypes.Count)];
        if (selectedType == MELEE_TAG) _currentMeleeCount++;
        else if (selectedType == RANGE_TAG) _currentRangeCount++;
        else if (selectedType == SUICIDE_TAG) _currentSuicideCount++;
        return selectedType;
    }

    private void SetTimeUntilSpawn() => _timeUntilSpawn = Random.Range(_minimumSpawnTime, _maximumSpawnTime);
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((_spawnAreaMin.x + _spawnAreaMax.x) / 2, (_spawnAreaMin.y + _spawnAreaMax.y) / 2, 0);
        Vector3 size = new Vector3(_spawnAreaMax.x - _spawnAreaMin.x, _spawnAreaMax.y - _spawnAreaMin.y, 0.1f);
        Gizmos.DrawWireCube(center, size);
        
        // Draw minimum player distance if player exists
        if (Application.isPlaying && _player != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawWireSphere(_player.transform.position, _minDistanceFromPlayer);
        }
    }
}