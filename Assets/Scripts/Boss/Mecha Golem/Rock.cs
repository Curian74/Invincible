using UnityEngine;

public class Rock : MonoBehaviour
{
    private Transform _player;
    private Animator _animator;
    private Rigidbody2D _rb;
    private BoxCollider2D _boxCollider;
    private float _speed = 8f;
    private float _lifeTime = 5f;
    private float _life = 0;
    public bool _isHoming = true;
    [SerializeField] private float damage = 5;
    public float multiplier = 1;
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _player = GameObject.FindWithTag("Player").transform;
    }

    void FixedUpdate()
    {     
        if (_player == null || !_player.gameObject.activeInHierarchy) return;
        if (_life < _lifeTime)
        {
            _life += Time.deltaTime;
            if (_isHoming)
            {
                _lifeTime = 1f;
                Vector2 direction = (_player.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                _rb.rotation = angle;
                _rb.linearVelocity = direction * _speed;
            }
        }
        else
        {
            gameObject.SetActive(false);
            _life = 0;
        }

    }

    public void SetDirection(float angle)
    {
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));    
        transform.rotation = Quaternion.Euler(0, 0, angle);
        _rb.linearVelocity = direction * _speed * 2;
        _lifeTime = 3f;
        gameObject.SetActive(true);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(damage * multiplier);
           
            gameObject.SetActive(false);
        }
    }
}
