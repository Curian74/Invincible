using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private GameObject _deathEffect;

    [Header("Enemy Type")]
    [SerializeField] private EnemyType _enemyType;
    public enum EnemyType { Melee, Range, Suicide, Egg }

    [Header("Combat Settings")]
    [SerializeField] private float _fireRate = 1.5f, _shootDistance = 5f;
    [SerializeField] private float _meleeAttackDistance = 1.2f, _meleeAttackCooldown = 1f;
    [SerializeField] private int _meleeDamage = 1;
    [SerializeField] private float _explosionRadius = 2f, _explosionDelay = 1.5f;
    [SerializeField] private float _explosionDetectionRadius = 1.5f;
    [SerializeField] private GameObject _explosionEffect;
    [SerializeField] private int _explosionBaseDamage = 50;
    [SerializeField] private float _minHealthPercentDamage = 10f, _maxHealthPercentDamage = 25f;

    [Header("Visual")]
    [SerializeField] private float _blinkRate = 0.2f;
    [SerializeField] private Color _normalColor = Color.white, _blinkColor = Color.red;

    [Header("Boundaries")]
    [SerializeField] private float _minX = -30f, _maxX = 47f, _minY = -20f, _maxY = 20f;

    private bool _canMove = true, _isCountingDown = false, _isAttacking = false;
    private bool _isBlinkingRed = false, _isExploding = false;
    private float _nextBlinkTime, _nextFireTime, _nextMeleeAttackTime;
    private Vector2 _targetDirection;
    private string _poolName;

    private Rigidbody2D _rb;
    private PlayerAware _playerAware;
    private SpriteRenderer _sprite;
    private Animator _anim;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerAware = GetComponent<PlayerAware>();
        _sprite = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _rb.gravityScale = 0;
        _targetDirection = transform.up;
        _poolName = GetEnemyPoolName();

        if (_normalColor == Color.white && _sprite != null)
            _normalColor = _sprite.color;
    }

    private void FixedUpdate()
    {
        if (_isCountingDown && _enemyType == EnemyType.Suicide)
        {
            HandleSuicideBlinking();
            return;
        }

        if (_playerAware.AwareOfPlayer)
        {
            _targetDirection = GetDirectionWithSeparation();

            if (_canMove)
            {
                float distToPlayer = Vector2.Distance(transform.position, _playerAware.PlayerPosition);
                bool shouldMove = true;

                switch (_enemyType)
                {
                    case EnemyType.Melee:
                        shouldMove = distToPlayer > _meleeAttackDistance;
                        break;
                    case EnemyType.Range:
                        shouldMove = distToPlayer > _shootDistance;
                        break;
                }

                _rb.linearVelocity = shouldMove ? _targetDirection * _speed : Vector2.zero;
                transform.rotation = Quaternion.Euler(0, _targetDirection.x < 0 ? 180 : 0, 0);

                if (_enemyType == EnemyType.Melee)
                    _anim.SetBool("walk", shouldMove && !_isAttacking);

                EnforceBoundaries();

                HandleTypeSpecificBehavior(distToPlayer);
            }
            else
            {
                _rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            _rb.linearVelocity = _targetDirection * _speed;
        }
    }

    private Vector2 GetDirectionWithSeparation()
    {
        Vector2 direction = _playerAware.DirectionToPlayer.normalized;

        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, 1.2f, LayerMask.GetMask("Enemy"));
        Vector2 separation = Vector2.zero;

        foreach (Collider2D enemy in nearbyEnemies)
        {
            if (enemy.gameObject != gameObject)
            {
                Vector2 pushDir = (Vector2)(transform.position - enemy.transform.position);
                if (pushDir.magnitude > 0)
                    separation += pushDir.normalized / pushDir.magnitude;
            }
        }

        if (separation != Vector2.zero)
            direction += separation * 0.5f;

        return direction.normalized;
    }

    private void EnforceBoundaries()
    {
        Vector2 nextPos = (Vector2)transform.position + _rb.linearVelocity * Time.fixedDeltaTime;
        Vector2 clampedPos = new Vector2(
            Mathf.Clamp(nextPos.x, _minX, _maxX),
            Mathf.Clamp(nextPos.y, _minY, _maxY)
        );

        if (nextPos != clampedPos)
        {
            transform.position = clampedPos;
            _rb.linearVelocity = Vector2.zero;
            if (nextPos.x != clampedPos.x || nextPos.y != clampedPos.y)
                _targetDirection = Random.insideUnitCircle.normalized;
        }
    }

    private void HandleTypeSpecificBehavior(float distToPlayer)
    {
        switch (_enemyType)
        {
            case EnemyType.Melee:
                if (distToPlayer <= _meleeAttackDistance && !_isAttacking && Time.time >= _nextMeleeAttackTime)
                    StartMeleeAttack();
                break;

            case EnemyType.Range:
                if (distToPlayer <= _shootDistance && Time.time >= _nextFireTime)
                    Fire();
                break;

            case EnemyType.Suicide:
                if (distToPlayer <= _explosionDetectionRadius && !_isCountingDown)
                    StartExplosionCountdown();
                break;
        }
    }

    private void StartMeleeAttack()
    {
        _isAttacking = true;
        _anim.SetTrigger("walk");
        _nextMeleeAttackTime = Time.time + _meleeAttackCooldown;
        Invoke(nameof(ExecuteMeleeAttack), 1.2f);
    }

    private void ExecuteMeleeAttack()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && Vector2.Distance(transform.position, player.transform.position) <= _meleeAttackDistance)
        {
            player.GetComponent<Health>()?.TakeDamage(_meleeDamage);
        }
        _isAttacking = false;
    }

    private void Fire()
    {
        _nextFireTime = Time.time + _fireRate;
        GameObject bullet = ObjectPool.Instance.SpawnFromPool("EnemyBullet", transform.position, Quaternion.identity);
        if (bullet != null)
        {
            bullet.SetActive(true);
            bullet.GetComponent<EnemyBullet>().SetDirection(_playerAware.DirectionToPlayer);
        }
    }

    private void StartExplosionCountdown()
    {
        if (_isCountingDown) return;

        _isCountingDown = true;
        _canMove = false;
        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Kinematic;

        foreach (Collider2D col in GetComponents<Collider2D>())
            if (!col.isTrigger) col.isTrigger = true;

        Invoke(nameof(Explode), _explosionDelay);
    }

    private void HandleSuicideBlinking()
    {
        if (Time.time < _nextBlinkTime) return;

        _nextBlinkTime = Time.time + _blinkRate;
        _isBlinkingRed = !_isBlinkingRed;
        _sprite.color = _isBlinkingRed ? _blinkColor : _normalColor;

        float remainingTime = _explosionDelay - (Time.time - (Time.time + _explosionDelay - _nextBlinkTime));
        if (remainingTime < _explosionDelay / 2)
            _blinkRate = 0.1f;
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

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit == null || !hit.gameObject.activeInHierarchy) continue;

            if (hit.CompareTag("Player"))
            {
                Health health = hit.GetComponent<Health>();
                if (health != null)
                {
                    int baseDamage = _explosionBaseDamage;
                    float percentDamage = Random.Range(_minHealthPercentDamage, _maxHealthPercentDamage);
                    int totalDamage = baseDamage + Mathf.RoundToInt(health.GetMaxHealth() * percentDamage / 100f);
                    health.TakeDamage(totalDamage);
                }
            }
            else if (hit.CompareTag("Enemy") && hit.gameObject != gameObject)
            {
                EnemyMovement enemy = hit.GetComponent<EnemyMovement>();
                if (enemy != null)
                {
                    // ScoreManager.Instance.AddScore(1);
                    if (enemy.GetEnemyType() == EnemyType.Suicide)
                        enemy.TriggerExplosion(Random.Range(0.1f, 0.3f));
                    else
                    {
                        enemy.DeathEffect();
                        enemy.gameObject.SetActive(false);
                        ObjectPool.Instance.ReturnToPool(enemy.GetOriginalPoolName(), enemy.gameObject);
                    }
                }
            }
        }

        _isExploding = false;
        gameObject.SetActive(false);
        ObjectPool.Instance.ReturnToPool(_poolName, gameObject);
    }

    public void TriggerExplosion(float delay)
    {
        if (_isCountingDown) return;

        _isCountingDown = true;
        _canMove = false;
        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Kinematic;

        foreach (Collider2D col in GetComponents<Collider2D>())
            if (!col.isTrigger) col.isTrigger = true;

        _sprite.color = _blinkColor;
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
            health.OnDeath += OnEnemyDeath;
    }

    private void OnDisable()
    {
        Health health = GetComponent<Health>();
        if (health != null)
            health.OnDeath -= OnEnemyDeath;
    }

    private void OnEnemyDeath()
    {
        // ScoreManager.Instance.AddScore(1);
        if (_enemyType == EnemyType.Suicide)
            Explode();
        else
        {
            DeathEffect();
            ObjectPool.Instance.ReturnToPool(_poolName, gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) => HandleCollision(collision.gameObject);
    private void OnCollisionEnter2D(Collision2D collision) => HandleCollision(collision.gameObject);

    private void HandleCollision(GameObject obj)
    {
        if (!obj.CompareTag("Player")) return;

        if (_enemyType == EnemyType.Suicide && !_isCountingDown)
            StartExplosionCountdown();
        else if (_enemyType == EnemyType.Melee && !_isAttacking && Time.time >= _nextMeleeAttackTime)
            StartMeleeAttack();
    }

    public void ResetEnemy()
    {
        CancelInvoke();

        Health health = GetComponent<Health>();
        if (health != null)
        {
            float amountToHeal = health.GetMaxHealth() - health.GetCurrentHealth();
            if (amountToHeal > 0) health.Heal(amountToHeal);
        }

        _targetDirection = transform.up;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;
        _isCountingDown = false;
        _isBlinkingRed = false;
        _isAttacking = false;
        _nextBlinkTime = 0f;
        _nextFireTime = 0f;
        _nextMeleeAttackTime = 0f;
        _sprite.color = _normalColor;
        _rb.bodyType = RigidbodyType2D.Kinematic;

        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            if (_enemyType == EnemyType.Suicide) col.isTrigger = false;
            col.enabled = true;
        }

        enabled = true;
        _canMove = true;
    }

    public void SetMovement(bool canMove) => _canMove = canMove;
    public EnemyType GetEnemyType() => _enemyType;
    public string GetOriginalPoolName() => _poolName;

    private string GetEnemyPoolName() => _enemyType switch
    {
        EnemyType.Range => "RangeEnemy",
        EnemyType.Melee => "MeleeEnemy",
        EnemyType.Suicide => "SuicideEnemy",
        EnemyType.Egg => "EggEnemy",
        _ => "Enemy"
    };

    private void OnDrawGizmosSelected()
    {
        if (_enemyType == EnemyType.Suicide)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _explosionDetectionRadius);
        }
        else if (_enemyType == EnemyType.Melee)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _meleeAttackDistance);
        }
    }
}