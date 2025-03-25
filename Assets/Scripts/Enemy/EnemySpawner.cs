using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _meleePrefab, _rangePrefab, _suicidePrefab, _eggPrefab, _bulletPrefab;
    [SerializeField] private float _minimumSpawnTime, _maximumSpawnTime;
    [SerializeField] private Vector2 _spawnAreaMin, _spawnAreaMax;
    [SerializeField] private int _meleeCount = 5, _rangeCount = 3, _suicideCount = 1, _eggCount = 1, _bulletCount = 30;
    [SerializeField] private float _minDistanceBetweenEnemies = 2f, _minDistanceFromPlayer = 4f;
    [SerializeField] private float _spawnCycleActiveTime = 45f;

    private const string MELEE_POOL = "MeleeEnemy", RANGE_POOL = "RangeEnemy",
                         SUICIDE_POOL = "SuicideEnemy", EGG_POOL = "EggEnemy",
                         BULLET_POOL = "EnemyBullet";

    private float _timeUntilSpawn, _spawnCycleTimer = 0f;
    private bool _isSpawningActive = true;
    private static int _cycleCount = 0;
    private int _currentMeleeCount, _currentRangeCount, _currentSuicideCount, _currentEggCount;
    private List<GameObject> _activeEnemies = new List<GameObject>();
    private GameObject _player;

    void Awake()
    {
        SetTimeUntilSpawn();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    void Start() => ValidateObjectPool();

    void Update()
    {
        UpdateSpawnCycle();
        if (_isSpawningActive)
        {
            _timeUntilSpawn -= Time.deltaTime;
            if (_timeUntilSpawn <= 0)
            {
                SpawnEnemy();
                SetTimeUntilSpawn();
            }
        }
        CleanupInactiveEnemies();
    }

    public static int GetCurrentCycle() => _cycleCount;

    private void UpdateSpawnCycle()
    {
        _spawnCycleTimer += Time.deltaTime;

        if (_isSpawningActive && _spawnCycleTimer >= _spawnCycleActiveTime)
        {
            _isSpawningActive = false;
            _spawnCycleTimer = 0f;
        }
        else if (!_isSpawningActive)
        {
            GameObject activeBoss = GameObject.FindGameObjectWithTag("Boss");

            if (activeBoss == null || !activeBoss.activeInHierarchy)
            {
                _isSpawningActive = true;
                _spawnCycleTimer = 0f;
                _cycleCount++;
                IncreasePoolSizes();
            }
        }
    }

    private void IncreasePoolSizes()
    {
        int[] increases = {
            Random.Range(3, 8),  // melee
            Random.Range(2, 6),  // range
            Random.Range(1, 5),  // suicide
            Random.Range(1, 3),  // egg
            Random.Range(15, 31) // bullet
        };

        _meleeCount += increases[0];
        _rangeCount += increases[1];
        _suicideCount += increases[2];
        _eggCount += increases[3];
        _bulletCount += increases[4];

        string[] poolTags = { MELEE_POOL, RANGE_POOL, SUICIDE_POOL, EGG_POOL, BULLET_POOL };
        GameObject[] prefabs = { _meleePrefab, _rangePrefab, _suicidePrefab, _eggPrefab, _bulletPrefab };

        for (int i = 0; i < poolTags.Length; i++)
            ExpandObjectPools(poolTags[i], increases[i], prefabs[i]);
    }

    private void ExpandObjectPools(string poolTag, int increaseAmount, GameObject prefab)
    {
        if (ObjectPool.Instance == null || !ObjectPool.Instance.poolDictionary.ContainsKey(poolTag))
            return;

        foreach (var pool in ObjectPool.Instance.pools)
        {
            if (pool.tag == poolTag)
            {
                pool.size += increaseAmount;
                break;
            }
        }

        Queue<GameObject> objectPool = ObjectPool.Instance.poolDictionary[poolTag];
        for (int i = 0; i < increaseAmount; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    private void CleanupInactiveEnemies()
    {
        int meleesActive = 0, rangesActive = 0, suicidesActive = 0, eggsActive = 0;

        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = _activeEnemies[i];
            if (enemy == null || !enemy.activeInHierarchy)
            {
                _activeEnemies.RemoveAt(i);
                continue;
            }

            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                switch (movement.GetEnemyType())
                {
                    case EnemyMovement.EnemyType.Melee: meleesActive++; break;
                    case EnemyMovement.EnemyType.Range: rangesActive++; break;
                    case EnemyMovement.EnemyType.Suicide: suicidesActive++; break;
                    case EnemyMovement.EnemyType.Egg: eggsActive++; break;
                }
            }
        }

        _currentMeleeCount = meleesActive;
        _currentRangeCount = rangesActive;
        _currentSuicideCount = suicidesActive;
        _currentEggCount = eggsActive;
    }

    private bool ValidateObjectPool() => ObjectPool.Instance.pools.Exists(pool =>
        pool.tag == MELEE_POOL || pool.tag == RANGE_POOL ||
        pool.tag == SUICIDE_POOL || pool.tag == EGG_POOL ||
        pool.tag == BULLET_POOL);

    private void SpawnEnemy()
    {
        string enemyPoolType = DetermineEnemyTypeToSpawn();
        Vector2 spawnPosition = FindValidSpawnPosition();

        if (spawnPosition == Vector2.zero)
        {
            SetTimeUntilSpawn();
            return;
        }

        GameObject enemy = ObjectPool.Instance.SpawnFromPool(enemyPoolType, spawnPosition, Quaternion.identity);
        if (enemy == null)
        {
            SetTimeUntilSpawn();
            return;
        }

        if (!enemy.activeInHierarchy)
            enemy.SetActive(true);

        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
            enemyMovement.ResetEnemy();

        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        enemy.tag = "Enemy";
        _activeEnemies.Add(enemy);
    }

    private Vector2 FindValidSpawnPosition()
    {
        const int MAX_ATTEMPTS = 30;
        for (int i = 0; i < MAX_ATTEMPTS; i++)
        {
            Vector2 randomPosition = new Vector2(
                Random.Range(_spawnAreaMin.x, _spawnAreaMax.x),
                Random.Range(_spawnAreaMin.y, _spawnAreaMax.y)
            );

            bool isTooCloseToPlayer = _player != null &&
                Vector2.Distance(randomPosition, _player.transform.position) < _minDistanceFromPlayer;

            bool isTooCloseToEnemies = _activeEnemies.Exists(enemy =>
                enemy != null && enemy.activeInHierarchy &&
                Vector2.Distance(randomPosition, enemy.transform.position) < _minDistanceBetweenEnemies);

            if (!isTooCloseToPlayer && !isTooCloseToEnemies)
                return randomPosition;
        }

        for (int i = 0; i < 10; i++)
        {
            Vector2 randomPosition = new Vector2(
                Random.Range(_spawnAreaMin.x, _spawnAreaMax.x),
                Random.Range(_spawnAreaMin.y, _spawnAreaMax.y)
            );

            if (_player == null || Vector2.Distance(randomPosition, _player.transform.position) >= _minDistanceFromPlayer)
                return randomPosition;
        }

        return Vector2.zero;
    }

    private string DetermineEnemyTypeToSpawn()
    {
        if (_currentMeleeCount >= _meleeCount && _currentRangeCount >= _rangeCount &&
            _currentSuicideCount >= _suicideCount && _currentEggCount >= _eggCount)
        {
            _currentMeleeCount = _currentRangeCount = _currentSuicideCount = _currentEggCount = 0;
        }

        List<string> availableTypes = new List<string>();
        if (_currentMeleeCount < _meleeCount) availableTypes.Add(MELEE_POOL);
        if (_currentRangeCount < _rangeCount) availableTypes.Add(RANGE_POOL);
        if (_currentSuicideCount < _suicideCount) availableTypes.Add(SUICIDE_POOL);
        if (_currentEggCount < _eggCount) availableTypes.Add(EGG_POOL);

        if (availableTypes.Count == 0)
            return DetermineEnemyTypeToSpawn();

        string selectedType = availableTypes[Random.Range(0, availableTypes.Count)];

        switch (selectedType)
        {
            case MELEE_POOL: _currentMeleeCount++; break;
            case RANGE_POOL: _currentRangeCount++; break;
            case SUICIDE_POOL: _currentSuicideCount++; break;
            case EGG_POOL: _currentEggCount++; break;
        }

        return selectedType;
    }

    private void SetTimeUntilSpawn() => _timeUntilSpawn = Random.Range(_minimumSpawnTime, _maximumSpawnTime);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((_spawnAreaMin.x + _spawnAreaMax.x) / 2, (_spawnAreaMin.y + _spawnAreaMax.y) / 2, 0);
        Vector3 size = new Vector3(_spawnAreaMax.x - _spawnAreaMin.x, _spawnAreaMax.y - _spawnAreaMin.y, 0.1f);
        Gizmos.DrawWireCube(center, size);

        if (Application.isPlaying && _player != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawWireSphere(_player.transform.position, _minDistanceFromPlayer);
        }
    }

    public void BurstSpawnOnDeath(Vector2 deathPosition, EggHealth.EnemyType enemyType)
    {
        int burstCount = Random.Range(50, 55);
        string[] enemyPools = { MELEE_POOL, RANGE_POOL, SUICIDE_POOL, EGG_POOL };

        for (int i = 0; i < burstCount; i++)
        {
            Vector2 spawnPosition = deathPosition + Random.insideUnitCircle * 3f;
            string enemyPoolType = enemyPools[Random.Range(0, enemyPools.Length)];
            GameObject enemy = ObjectPool.Instance.SpawnFromPool(enemyPoolType, spawnPosition, Quaternion.identity);

            if (enemy != null)
            {
                enemy.SetActive(true);
                EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
                if (enemyMovement != null)
                    enemyMovement.ResetEnemy();
                _activeEnemies.Add(enemy);
            }
        }
    }
}