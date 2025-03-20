using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _screenBorder;
    [SerializeField] private GameObject _deathEffect;

    [Header("Enemy Type Settings")]
    [SerializeField] private EnemyType _enemyType;

    [Header("Shooting Settings")]
    [SerializeField] private float _fireRate = 1.5f;
    [SerializeField] private float _shootDistance = 5f;
    private float _nextFireTime;

    [Header("Explosion Settings")]
    [SerializeField] private float _explosionRadius = 2f;
    [SerializeField] private float _explosionDelay = 1.5f;
    [SerializeField] private GameObject _explosionEffect;
    [SerializeField] private float _explosionDetectionRadius = 1.5f;
    [SerializeField] private int _explosionBaseDamage = 50;
    [SerializeField] private float _minHealthPercentDamage = 10f;
    [SerializeField] private float _maxHealthPercentDamage = 25f;
    private bool _isExploding = false;

    [Header("Melee Settings")]
    [SerializeField] private float _meleeAttackDistance = 1.2f;
    [SerializeField] private float _meleeAttackCooldown = 1f;
    [SerializeField] private int _meleeDamage = 1;
    private float _nextMeleeAttackTime = 0f;

    [Header("Visual Settings")]
    [SerializeField] private float _blinkRate = 0.2f;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _blinkColor = Color.red;
    [Header("Obstacle Avoidance")]
    [SerializeField] private float _obstacleCheckCircleRadius = 0.5f;
    [SerializeField] private float _obstacleCheckDistance = 1.5f;
    [SerializeField] private LayerMask _obstacleLayerMask;
    private bool _canMove = true;
    private bool _isCountingDown = false;
    private bool _isBlinkingRed = false;
    private float _nextBlinkTime;

    private Rigidbody2D _rigidbody;
    private PlayerAware _playerAwareness;
    private Vector2 _targetDirection;
    private Camera _camera;
    private SpriteRenderer _spriteRenderer;
    private string _originalPoolName;
    public enum EnemyType
    {
        Melee,
        Range,
        Suicide
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAwareness = GetComponent<PlayerAware>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _camera = Camera.main;

        _targetDirection = transform.up;
        _rigidbody.gravityScale = 0;
        _originalPoolName = GetEnemyPoolName();

        if (_normalColor == Color.white && _spriteRenderer != null)
        {
            _normalColor = _spriteRenderer.color;
        }
    }

    private void FixedUpdate()
    {
        if (_isCountingDown && _enemyType == EnemyType.Suicide)
        {
            HandleBlinking();
            return;
        }

        UpdateTargetDirection();
        RotateTowardsTarget();

        if (_canMove)
        {
            SetVelocity();
        }
        else
        {
            _rigidbody.linearVelocity = Vector2.zero;
        }

        if (_enemyType == EnemyType.Melee && _playerAwareness.AwareOfPlayer && IsAtMeleeRange())
        {
            TryMeleeAttack();
        }

        if (_enemyType == EnemyType.Suicide && _playerAwareness.AwareOfPlayer && !_isCountingDown)
        {
            CheckForExplosion();
        }
    }

    private void Update()
    {
        if (_enemyType == EnemyType.Range && CanFire())
        {
            Fire();
        }
    }

private void UpdateTargetDirection()
{
    if (_playerAwareness != null && _playerAwareness.AwareOfPlayer)
    {
        Vector2 directionToPlayer = _playerAwareness.DirectionToPlayer.normalized;

        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, 1.2f, LayerMask.GetMask("Enemy"));
        Vector2 separationForce = Vector2.zero;

        foreach (Collider2D enemy in nearbyEnemies)
        {
            if (enemy.gameObject != this.gameObject)
            {
                Vector2 pushDirection = (Vector2)(transform.position - enemy.transform.position);
                separationForce += pushDirection.normalized / pushDirection.magnitude;
            }
        }
        if (separationForce != Vector2.zero)
        {
            directionToPlayer += separationForce * 0.5f;
        }
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, _obstacleCheckCircleRadius, directionToPlayer, _obstacleCheckDistance, _obstacleLayerMask);
        if (hit.collider != null)
        {
            Vector2 avoidDirection = Vector2.Perpendicular(directionToPlayer).normalized;
            directionToPlayer += avoidDirection * 0.75f;
        }

        _targetDirection = directionToPlayer.normalized;
    }
}

    private void RotateTowardsTarget()
    {
        if (_canMove && !(_enemyType == EnemyType.Melee && IsAtMeleeRange()))
        {
            transform.rotation = Quaternion.Euler(0, _targetDirection.x < 0 ? 180 : 0, 0);
        }
    }

    private void SetVelocity()
    {
        float distanceToPlayer = _playerAwareness.AwareOfPlayer ?
            Vector2.Distance(transform.position, _playerAwareness.PlayerPosition) : float.MaxValue;

        if (_enemyType == EnemyType.Melee && _playerAwareness.AwareOfPlayer)
        {
            _rigidbody.linearVelocity = distanceToPlayer <= _meleeAttackDistance ?
                Vector2.zero : _targetDirection.normalized * _speed;
        }
        else if (_enemyType == EnemyType.Range && _playerAwareness.AwareOfPlayer)
        {
            if (distanceToPlayer > _shootDistance + 0.5f)
                _rigidbody.linearVelocity = _targetDirection.normalized * _speed;
            else if (distanceToPlayer < _shootDistance - 0.5f)
                _rigidbody.linearVelocity = -_targetDirection.normalized * _speed;
            else
                _rigidbody.linearVelocity = Vector2.zero;
        }
        else
        {
            _rigidbody.linearVelocity = _targetDirection.normalized * _speed;
        }
    }

    private bool IsAtMeleeRange()
    {
        if (!_playerAwareness.AwareOfPlayer) return false;
        return Vector2.Distance(transform.position, _playerAwareness.PlayerPosition) <= _meleeAttackDistance;
    }

    private void TryMeleeAttack()
    {
        if (Time.time >= _nextMeleeAttackTime)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Health playerHealth = player.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(_meleeDamage);
                }
            }
            _nextMeleeAttackTime = Time.time + _meleeAttackCooldown;
        }
    }

    private bool CanFire()
    {
        if (!_playerAwareness.AwareOfPlayer || Time.time < _nextFireTime)
            return false;

        return Vector2.Distance(transform.position, _playerAwareness.PlayerPosition) <= _shootDistance;
    }

    private void Fire()
    {
        _nextFireTime = Time.time + _fireRate;
        GameObject bullet = ObjectPool.Instance.SpawnFromPool("EnemyBullet", transform.position, Quaternion.identity);
        if (bullet != null)
        {
            bullet.SetActive(true);
            Vector2 direction = _playerAwareness.AwareOfPlayer ?
                _playerAwareness.DirectionToPlayer : _targetDirection;
            bullet.GetComponent<EnemyBullet>().SetDirection(direction);
        }
    }

    private void CheckForExplosion()
    {
        if (_playerAwareness.AwareOfPlayer)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, _playerAwareness.PlayerPosition);
            if (distanceToPlayer <= _explosionDetectionRadius)
            {
                StartExplosionCountdown();
            }
        }
    }

    private void StartExplosionCountdown()
    {
        if (_isCountingDown) return;

        _isCountingDown = true;
        _canMove = false;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;

        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            if (!col.isTrigger)
            {
                col.isTrigger = true;
            }
        }

        CancelInvoke(nameof(Explode));
        Invoke(nameof(Explode), _explosionDelay);
    }

    private void HandleBlinking()
    {
        if (Time.time >= _nextBlinkTime)
        {
            _nextBlinkTime = Time.time + _blinkRate;

            if (_spriteRenderer != null)
            {
                _isBlinkingRed = !_isBlinkingRed;
                _spriteRenderer.color = _isBlinkingRed ? _blinkColor : _normalColor;
            }

            float remainingTime = _explosionDelay - (Time.time - (Time.time + _explosionDelay - _nextBlinkTime));
            if (remainingTime < _explosionDelay / 2)
            {
                _blinkRate = 0.1f;
            }
        }
    }

    private void Explode()
    {
        if (!gameObject.activeInHierarchy || _isExploding) return;

        _isExploding = true;
        CancelInvoke();

        if (_explosionEffect != null)
        {
            GameObject explosion = Instantiate(_explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 5f);
        }

        DeathEffect();

        Collider2D[] objectsHit = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);
        foreach (Collider2D obj in objectsHit)
        {
            if (obj == null || !obj.gameObject.activeInHierarchy) continue;

            if (obj.CompareTag("Player"))
            {
                Health playerHealth = obj.GetComponent<Health>();
                if (playerHealth != null)
                {
                    int baseDamage = _explosionBaseDamage;
                    float percentDamage = Random.Range(_minHealthPercentDamage, _maxHealthPercentDamage);
                    float maxHealth = playerHealth.GetMaxHealth();
                    int percentBasedDamage = Mathf.RoundToInt(maxHealth * percentDamage / 100f);
                    int totalDamage = baseDamage + percentBasedDamage;

                    playerHealth.TakeDamage(totalDamage);
                }
            }
            else if (obj.CompareTag("Enemy") && obj.gameObject != gameObject)
            {
                EnemyMovement enemyMovement = obj.GetComponent<EnemyMovement>();
                if (enemyMovement != null)
                {
                    ScoreManager.Instance.AddScore(1);

                    if (enemyMovement.GetEnemyType() == EnemyType.Suicide)
                    {
                        float randomDelay = Random.Range(0.1f, 0.3f);
                        enemyMovement.TriggerExplosion(randomDelay);
                    }
                    else
                    {
                        enemyMovement.DeathEffect();
                        obj.gameObject.SetActive(false);
                        ObjectPool.Instance.ReturnToPool(enemyMovement.GetOriginalPoolName(), obj.gameObject);
                    }
                }
            }
        }

        _isExploding = false;
        gameObject.SetActive(false);
        ObjectPool.Instance.ReturnToPool(_originalPoolName, gameObject);
    }

    public void TriggerExplosion(float delay)
    {
        if (_isCountingDown) return;

        _isCountingDown = true;
        _canMove = false;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;

        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            if (!col.isTrigger)
            {
                col.isTrigger = true;
            }
        }

        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _blinkColor;
        }

        CancelInvoke(nameof(Explode));
        Invoke(nameof(Explode), delay);
    }

    public void DeathEffect()
    {
        if (_deathEffect != null)
        {
            GameObject effect = Instantiate(_deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }

    private void OnEnable()
    {
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath += OnEnemyDeath;
        }
    }

    private void OnDisable()
    {
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath -= OnEnemyDeath;
        }
    }

    private void OnEnemyDeath()
    {
        ScoreManager.Instance.AddScore(1);

        if (_enemyType == EnemyType.Suicide)
        {
            Explode();
        }
        else
        {
            DeathEffect();
            ObjectPool.Instance.ReturnToPool(_originalPoolName, gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject collisionObject)
    {
        if (collisionObject.CompareTag("Player"))
        {
            if (_enemyType == EnemyType.Suicide)
            {
                if (!_isCountingDown) StartExplosionCountdown();
            }
            else if (_enemyType == EnemyType.Melee)
            {
                Health playerHealth = collisionObject.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(_meleeDamage);
                }
            }
        }
    }

    public void ResetEnemy()
    {
        CancelInvoke();

        Health health = GetComponent<Health>();
        if (health != null)
        {
            float currentHealth = health.GetCurrentHealth();
            float maxHealth = health.GetMaxHealth();
            float amountToHeal = maxHealth - currentHealth;

            if (amountToHeal > 0)
            {
                health.Heal(amountToHeal);
            }
        }

        _targetDirection = transform.up;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _isCountingDown = false;
        _isBlinkingRed = false;
        _nextBlinkTime = 0f;
        _nextFireTime = 0f;
        _nextMeleeAttackTime = 0f;

        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _normalColor;
        }

        _rigidbody.bodyType = RigidbodyType2D.Kinematic;

        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            if (_enemyType == EnemyType.Suicide)
            {
                col.isTrigger = false;
            }
            col.enabled = true;
        }

        enabled = true;
        _canMove = true;
    }

    public void SetMovement(bool canMove)
    {
        _canMove = canMove;
    }

    public EnemyType GetEnemyType()
    {
        return _enemyType;
    }

    public string GetOriginalPoolName()
    {
        return _originalPoolName;
    }

    private string GetEnemyPoolName()
    {
        switch (_enemyType)
        {
            case EnemyType.Range: return "RangeEnemy";
            case EnemyType.Melee: return "MeleeEnemy";
            case EnemyType.Suicide: return "SuicideEnemy";
            default: return "Enemy";
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_enemyType == EnemyType.Suicide)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _explosionDetectionRadius);
        }

        if (_enemyType == EnemyType.Melee)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _meleeAttackDistance);
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _obstacleCheckCircleRadius);
        Gizmos.DrawRay(transform.position, transform.up * _obstacleCheckDistance);
    }
}