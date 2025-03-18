using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _screenBorder;
    [SerializeField] private bool _canMove = true;
    [SerializeField] private bool _canShoot = false;
    [SerializeField] private GameObject _deathEffect;

    [Header("Enemy Type Settings")]
    [SerializeField] private bool _isMelee;
    [SerializeField] private bool _isSuicideBomber;

    [Header("Shooting Settings")]
    [SerializeField] private float _fireRate = 1.5f;
    [SerializeField] private float _shootDistance = 5f;
    private float _nextFireTime;

    [Header("Explosion Settings")]
    [SerializeField] private float _explosionRadius = 2f;
    [SerializeField] private float _explosionDelay = 1.5f;
    [SerializeField] private GameObject _explosionEffect;
    [SerializeField] private float _explosionDetectionRadius = 1.5f;
    [SerializeField] private bool _instantExplosionOnContact = true;

    [SerializeField] private float _blinkRate = 0.2f;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _blinkColor = Color.red;
    private SpriteRenderer _spriteRenderer;
    private float _nextBlinkTime;
    private bool _isBlinkingRed = false;

    private Rigidbody2D _rigidbody;
    private PlayerAware _playerAwareness;
    private Vector2 _targetDirection;
    private Camera _camera;
    private bool _isCountingDown = false;

    [SerializeField] private float _meleeAttackDistance = 1.2f;
    [SerializeField] private float _meleeAttackCooldown = 1f;
    [SerializeField] private int _meleeDamage = 1;
    private float _nextMeleeAttackTime = 0f;

    public enum EnemyType
    {
        Melee,
        Range,
        Suicide
    }

    [SerializeField] private EnemyType _enemyType;

    private string _originalPoolName;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAwareness = GetComponent<PlayerAware>();
        _targetDirection = transform.up;
        _camera = Camera.main;
        _rigidbody.gravityScale = 0;
        _spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateEnemyTypeFromBooleans();

        _originalPoolName = GetEnemyPoolName(gameObject);

        if (_normalColor == Color.white && _spriteRenderer != null)
        {
            _normalColor = _spriteRenderer.color;
        }
    }

    private void UpdateEnemyTypeFromBooleans()
    {
        if (_isSuicideBomber)
        {
            _enemyType = EnemyType.Suicide;
        }
        else if (_isMelee)
        {
            _enemyType = EnemyType.Melee;
        }
        else
        {
            _enemyType = EnemyType.Range;
        }

        _isMelee = _enemyType == EnemyType.Melee;
        _isSuicideBomber = _enemyType == EnemyType.Suicide;
        _canShoot = _enemyType == EnemyType.Range;
    }

    public EnemyType GetEnemyType()
    {
        return _enemyType;
    }

    public string GetOriginalPoolName()
    {
        return _originalPoolName;
    }

    private void FixedUpdate()
    {
        if (_isCountingDown && _isSuicideBomber)
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

        if (_isMelee && _playerAwareness.AwareOfPlayer && IsAtMeleeRange())
        {
            TryMeleeAttack();
        }

        if (_isSuicideBomber && _playerAwareness.AwareOfPlayer && !_isCountingDown)
        {
            CheckForExplosion();
        }
    }

    private void Update()
    {
        if (_canShoot && !_isMelee && !_isSuicideBomber && CanFire())
        {
            Fire();
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

    private void UpdateTargetDirection()
    {
        HandlePlayerTargeting();
        HandleEnemyOffScreen();
    }

    private void HandlePlayerTargeting()
    {
        if (_playerAwareness == null || !_playerAwareness.AwareOfPlayer) return;

        if (!_playerAwareness.PlayerPosition.Equals(Vector2.zero))
        {
            _targetDirection = _playerAwareness.DirectionToPlayer;
        }
    }

    private void StartExplosionCountdown()
    {
        if (_isCountingDown) return;

        _isCountingDown = true;
        _canMove = false;
        _rigidbody.linearVelocity = Vector2.zero;
        CancelInvoke(nameof(Explode));
        Invoke(nameof(Explode), _explosionDelay);
    }

    private void HandleEnemyOffScreen()
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);

        if ((screenPosition.x < _screenBorder && _targetDirection.x < 0) ||
            (screenPosition.x > _camera.pixelWidth - _screenBorder && _targetDirection.x > 0))
        {
            _targetDirection = new Vector2(-_targetDirection.x, _targetDirection.y);
        }

        if ((screenPosition.y < _screenBorder && _targetDirection.y < 0) ||
            (screenPosition.y > _camera.pixelHeight - _screenBorder && _targetDirection.y > 0))
        {
            _targetDirection = new Vector2(_targetDirection.x, -_targetDirection.y);
        }
    }

    public void ResetEnemy()
    {
        CancelInvoke();

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

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        enabled = true;
        _canMove = true;
        _canShoot = _enemyType == EnemyType.Range;
    }

    private void RotateTowardsTarget()
    {
        if (_canMove && !(_isMelee && IsAtMeleeRange()))
        {
            if (_targetDirection.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (_targetDirection.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    private bool IsAtMeleeRange()
    {
        if (!_playerAwareness.AwareOfPlayer) return false;

        float distanceToPlayer = Vector2.Distance(transform.position, _playerAwareness.PlayerPosition);
        return distanceToPlayer <= _meleeAttackDistance;
    }

    private void SetVelocity()
    {
        if (_isMelee && _playerAwareness.AwareOfPlayer)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, _playerAwareness.PlayerPosition);

            if (distanceToPlayer <= _meleeAttackDistance)
            {
                _rigidbody.linearVelocity = Vector2.zero;
            }
            else
            {
                _rigidbody.linearVelocity = _targetDirection.normalized * _speed;
            }
        }
        else if (!_isMelee && _canShoot && _playerAwareness.AwareOfPlayer)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, _playerAwareness.PlayerPosition);

            if (distanceToPlayer > _shootDistance + 0.5f)
            {
                _rigidbody.linearVelocity = _targetDirection.normalized * _speed;
            }
            else if (distanceToPlayer < _shootDistance - 0.5f)
            {
                _rigidbody.linearVelocity = -_targetDirection.normalized * _speed;
            }
            else
            {
                _rigidbody.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            _rigidbody.linearVelocity = _targetDirection.normalized * _speed;
        }
    }

    private bool CanFire()
    {
        if (!_playerAwareness.AwareOfPlayer || Time.time < _nextFireTime)
            return false;

        float distanceToPlayer = Vector2.Distance(transform.position, _playerAwareness.PlayerPosition);
        return distanceToPlayer <= _shootDistance;
    }

    private void Fire()
    {
        _nextFireTime = Time.time + _fireRate;
        GameObject bullet = ObjectPool.Instance.SpawnFromPool("EnemyBullet", transform.position, Quaternion.identity);
        if (bullet != null)
        {
            bullet.SetActive(true);

            if (_playerAwareness != null && _playerAwareness.AwareOfPlayer)
            {
                Vector2 currentDirectionToPlayer = _playerAwareness.DirectionToPlayer;
                bullet.GetComponent<EnemyBullet>().SetDirection(currentDirectionToPlayer);
            }
            else
            {
                bullet.GetComponent<EnemyBullet>().SetDirection(_targetDirection);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_isSuicideBomber && _instantExplosionOnContact)
            {
                CancelInvoke(nameof(Explode));
                Explode();
            }
            else if (_isMelee)
            {
                Health playerHealth = collision.gameObject.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(_meleeDamage);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_isSuicideBomber && _instantExplosionOnContact)
            {
                CancelInvoke(nameof(Explode));
                Explode();
            }
            else if (_isMelee)
            {
                Health playerHealth = collision.gameObject.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(_meleeDamage);
                }
            }
        }
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
        if (!gameObject.activeInHierarchy) return;

        CancelInvoke();

        DeathEffect();

        if (_explosionEffect != null)
        {
            GameObject explosion = Instantiate(_explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 5f);
        }

        Collider2D[] objectsHit = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);
        foreach (Collider2D obj in objectsHit)
        {
            if (obj == null || !obj.gameObject.activeInHierarchy) continue;

            if (obj.CompareTag("Player"))
            {
                Health playerHealth = obj.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(50);
                }
            }
            else if (obj.CompareTag("Enemy"))
            {
                if (obj.gameObject == gameObject) continue;

                EnemyMovement enemyMovement = obj.GetComponent<EnemyMovement>();
                if (enemyMovement != null)
                {
                    enemyMovement.DeathEffect();

                    obj.gameObject.SetActive(false);
                    string enemyType = enemyMovement.GetOriginalPoolName();
                    ObjectPool.Instance.ReturnToPool(enemyType, obj.gameObject);
                }
            }
        }

        gameObject.SetActive(false);
        ObjectPool.Instance.ReturnToPool(_originalPoolName, gameObject);
    }

    private string GetEnemyPoolName(GameObject enemy)
    {
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            switch (enemyMovement.GetEnemyType())
            {
                case EnemyType.Range:
                    return "RangeEnemy";
                case EnemyType.Melee:
                    return "MeleeEnemy";
                case EnemyType.Suicide:
                    return "SuicideEnemy";
            }
        }
        return "Enemy";
    }

    private void OnDrawGizmosSelected()
    {
        if (_isSuicideBomber)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _explosionDetectionRadius);
        }

        if (_isMelee)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _meleeAttackDistance);
        }
    }

    public void SetMovement(bool canMove)
    {
        _canMove = canMove;
    }

    public void SetShooting(bool canShoot)
    {
        _canShoot = canShoot;
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
        DeathEffect();

        ObjectPool.Instance.ReturnToPool(_originalPoolName, gameObject);
    }
    public void DeathEffect()
    {
        if (_deathEffect != null)
        {
            GameObject effect = Instantiate(_deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }
}