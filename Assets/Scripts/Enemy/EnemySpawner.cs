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
    
    // Định nghĩa tên pool thay vì sử dụng tag
    private const string MELEE_POOL = "MeleeEnemy";
    private const string RANGE_POOL = "RangeEnemy";
    private const string SUICIDE_POOL = "SuicideEnemy";
    
    private float _timeUntilSpawn;
    private List<GameObject> _activeEnemies = new List<GameObject>();
    [SerializeField] private float _minDistanceBetweenEnemies = 2f;
    [SerializeField] private float _minDistanceFromPlayer = 4f;
    
    private GameObject _player;
    
    // Thêm biến để debug
    [SerializeField] private bool _debugMode = false;

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
        // Cập nhật danh sách kẻ địch hoạt động và kiểm tra các kẻ địch không hoạt động
        CleanupInactiveEnemies();
    }
    
    private void CleanupInactiveEnemies()
    {
        // Đếm số lượng kẻ địch theo loại để điều chỉnh nếu cần
        int meleesActive = 0, rangesActive = 0, suicidesActive = 0;
        
        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = _activeEnemies[i];
            if (enemy == null || !enemy.activeInHierarchy)
            {
                _activeEnemies.RemoveAt(i);
                continue;
            }
            
            // Đếm kẻ địch theo loại
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                switch (movement.GetEnemyType())
                {
                    case EnemyMovement.EnemyType.Melee: meleesActive++; break;
                    case EnemyMovement.EnemyType.Range: rangesActive++; break;
                    case EnemyMovement.EnemyType.Suicide: suicidesActive++; break;
                }
            }
        }
        
        // Cập nhật số lượng kẻ địch hiện tại
        _currentMeleeCount = meleesActive;
        _currentRangeCount = rangesActive;
        _currentSuicideCount = suicidesActive;
        
        if (_debugMode)
        {
            Debug.Log($"Active enemies: Melee={meleesActive}, Range={rangesActive}, Suicide={suicidesActive}");
        }
    }

    private void ValidateObjectPool()
    {
        bool meelePoolExists = false, rangePoolExists = false, suicidePoolExists = false;
        foreach (ObjectPool.Pool pool in ObjectPool.Instance.pools)
        {
            if (pool.tag == MELEE_POOL) meelePoolExists = true;
            if (pool.tag == RANGE_POOL) rangePoolExists = true;
            if (pool.tag == SUICIDE_POOL) suicidePoolExists = true;
        }
        
        // Nếu debug mode, log thông tin về pool
        if (_debugMode)
        {
            Debug.Log($"Object pools validated: Melee={meelePoolExists}, Range={rangePoolExists}, Suicide={suicidePoolExists}");
        }
    }

    private void SpawnEnemy()
    {
        string enemyPoolType = DetermineEnemyTypeToSpawn();
        
        if (_debugMode)
        {
            Debug.Log($"Trying to spawn enemy of type: {enemyPoolType}");
        }
        
        Vector2 spawnPosition = FindValidSpawnPosition();
        if (spawnPosition == Vector2.zero) 
        { 
            if (_debugMode) Debug.Log("No valid spawn position found.");
            SetTimeUntilSpawn(); 
            return; 
        }
        
        GameObject enemy = ObjectPool.Instance.SpawnFromPool(enemyPoolType, spawnPosition, Quaternion.identity);
        if (enemy == null) 
        {
            if (_debugMode) Debug.Log($"Failed to spawn from pool: {enemyPoolType}");
            SetTimeUntilSpawn(); 
            return; 
        }
        
        if (!enemy.activeInHierarchy)
        {
            enemy.SetActive(true);
            if (_debugMode) Debug.Log($"Activated enemy from pool: {enemyPoolType}");
        }
        
        // Reset enemy component
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.ResetEnemy();
        }
        
        // Make sure enemy's layer collision is configured to allow passing through other enemies
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Set enemy's collision detection mode
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
        
        // Đảm bảo tag là "Enemy" cho tất cả các loại kẻ địch
        enemy.tag = "Enemy";
        
        _activeEnemies.Add(enemy);
        
        if (_debugMode)
        {
            Debug.Log($"Successfully spawned enemy of type: {enemyPoolType}, Active enemies: {_activeEnemies.Count}");
        }
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
        // Reset count nếu đã đủ tất cả loại kẻ địch
        if (_currentMeleeCount >= _meleeCount && _currentRangeCount >= _rangeCount && _currentSuicideCount >= _suicideCount)
        {
            _currentMeleeCount = 0;
            _currentRangeCount = 0;
            _currentSuicideCount = 0;
        }
        
        List<string> availableTypes = new List<string>();
        if (_currentMeleeCount < _meleeCount) availableTypes.Add(MELEE_POOL);
        if (_currentRangeCount < _rangeCount) availableTypes.Add(RANGE_POOL);
        if (_currentSuicideCount < _suicideCount) availableTypes.Add(SUICIDE_POOL);
        
        // Nếu không có loại nào khả dụng (hiếm khi xảy ra), thử lại
        if (availableTypes.Count == 0)
        {
            if (_debugMode) Debug.Log("No available enemy types to spawn!");
            return DetermineEnemyTypeToSpawn();
        }
        
        string selectedType = availableTypes[Random.Range(0, availableTypes.Count)];
        if (selectedType == MELEE_POOL) _currentMeleeCount++;
        else if (selectedType == RANGE_POOL) _currentRangeCount++;
        else if (selectedType == SUICIDE_POOL) _currentSuicideCount++;
        
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