using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _meleePrefab;
    [SerializeField] private GameObject _rangePrefab;
    [SerializeField] private GameObject _suicidePrefab;
    [SerializeField] private GameObject _eggPrefab;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _minimumSpawnTime;
    [SerializeField] private float _maximumSpawnTime;

    [SerializeField] private Vector2 _spawnAreaMin;
    [SerializeField] private Vector2 _spawnAreaMax;

    [SerializeField] private int _meleeCount = 5;
    [SerializeField] private int _rangeCount = 3;
    [SerializeField] private int _suicideCount = 1;
    [SerializeField] private int _eggCount = 1;
    [SerializeField] private int _bulletCount = 30;
    private int _currentMeleeCount = 0;
    private int _currentRangeCount = 0;
    private int _currentSuicideCount = 0;
    private int _currentEggCount = 0;

    private const string MELEE_POOL = "MeleeEnemy";
    private const string RANGE_POOL = "RangeEnemy";
    private const string SUICIDE_POOL = "SuicideEnemy";
    private const string EGG_POOL = "EggEnemy";
    private const string BULLET_POOL = "EnemyBullet";
    private float _timeUntilSpawn;
    private List<GameObject> _activeEnemies = new List<GameObject>();
    [SerializeField] private float _minDistanceBetweenEnemies = 2f;
    [SerializeField] private float _minDistanceFromPlayer = 4f;

    private GameObject _player;

    [SerializeField] private float _spawnCycleActiveTime = 45f;
    [SerializeField] private float _spawnCyclePauseTime = 30f;
    private float _spawnCycleTimer = 0f;
    private bool _isSpawningActive = true;
    private int _cycleCount = 0;
    
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

    private void UpdateSpawnCycle()
    {
        _spawnCycleTimer += Time.deltaTime;

        if (_isSpawningActive && _spawnCycleTimer >= _spawnCycleActiveTime)
        {
            _isSpawningActive = false;
            _spawnCycleTimer = 0f;
        }
        else if (!_isSpawningActive && _spawnCycleTimer >= _spawnCyclePauseTime)
        {
            _isSpawningActive = true;
            _spawnCycleTimer = 0f;
            _cycleCount++;

            IncreasePoolSizes();
        }
    }

    private void IncreasePoolSizes()
    {
        int meleeIncrease = Random.Range(3, 8);
        int rangeIncrease = Random.Range(2, 6);
        int suicideIncrease = Random.Range(1, 5);
        int eggIncrease = Random.Range(1, 3);
        int bulletIncrease = Random.Range(15, 31);

        _meleeCount += meleeIncrease;
        _rangeCount += rangeIncrease;
        _suicideCount += suicideIncrease;
        _eggCount += eggIncrease;
        _bulletCount += bulletIncrease;

        ExpandObjectPools(MELEE_POOL, meleeIncrease, _meleePrefab);
        ExpandObjectPools(RANGE_POOL, rangeIncrease, _rangePrefab);
        ExpandObjectPools(SUICIDE_POOL, suicideIncrease, _suicidePrefab);
        ExpandObjectPools(EGG_POOL, eggIncrease, _eggPrefab);
        ExpandObjectPools(BULLET_POOL, bulletIncrease, _bulletPrefab);
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

    private bool ValidateObjectPool()
    {
        return ObjectPool.Instance.pools.Exists(pool =>
            pool.tag == MELEE_POOL || pool.tag == RANGE_POOL ||
            pool.tag == SUICIDE_POOL || pool.tag == EGG_POOL ||
            pool.tag == BULLET_POOL);
    }

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
        {
            enemy.SetActive(true);
        }

        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.ResetEnemy();
        }

        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        enemy.tag = "Enemy";

        _activeEnemies.Add(enemy);
    }

    private Vector2 FindValidSpawnPosition()
    {
        const int MAX_ATTEMPTS = 30;
        for (int i = 0; i < MAX_ATTEMPTS; i++)
        {
            Vector2 randomPosition = new Vector2(Random.Range(_spawnAreaMin.x, _spawnAreaMax.x), Random.Range(_spawnAreaMin.y, _spawnAreaMax.y));

            bool isTooCloseToPlayer = _player != null && Vector2.Distance(randomPosition, _player.transform.position) < _minDistanceFromPlayer;

            bool isTooCloseToEnemies = _activeEnemies.Exists(enemy =>
                enemy != null &&
                enemy.activeInHierarchy &&
                Vector2.Distance(randomPosition, enemy.transform.position) < _minDistanceBetweenEnemies);

            if (!isTooCloseToPlayer && !isTooCloseToEnemies)
                return randomPosition;
        }

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
        if (_currentMeleeCount >= _meleeCount && _currentRangeCount >= _rangeCount &&
            _currentSuicideCount >= _suicideCount && _currentEggCount >= _eggCount)
        {
            _currentMeleeCount = 0;
            _currentRangeCount = 0;
            _currentSuicideCount = 0;
            _currentEggCount = 0;
        }

        List<string> availableTypes = new List<string>();
        if (_currentMeleeCount < _meleeCount) availableTypes.Add(MELEE_POOL);
        if (_currentRangeCount < _rangeCount) availableTypes.Add(RANGE_POOL);
        if (_currentSuicideCount < _suicideCount) availableTypes.Add(SUICIDE_POOL);
        if (_currentEggCount < _eggCount) availableTypes.Add(EGG_POOL);

        if (availableTypes.Count == 0)
        {
            return DetermineEnemyTypeToSpawn();
        }

        string selectedType = availableTypes[Random.Range(0, availableTypes.Count)];

        if (selectedType == MELEE_POOL) _currentMeleeCount++;
        else if (selectedType == RANGE_POOL) _currentRangeCount++;
        else if (selectedType == SUICIDE_POOL) _currentSuicideCount++;
        else if (selectedType == EGG_POOL) _currentEggCount++;

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
        int burstCount = Random.Range(2, 5);

        List<string> enemyPools = new List<string> { MELEE_POOL, RANGE_POOL, SUICIDE_POOL, EGG_POOL };

        for (int i = 0; i < burstCount; i++)
        {
            Vector2 spawnPosition = deathPosition + Random.insideUnitCircle * 1.5f;

            string enemyPoolType = enemyPools[Random.Range(0, enemyPools.Count)];

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