using UnityEngine;

public class Rock : MonoBehaviour
{
    private Transform _player;
    private Animator _animator;
    private Rigidbody2D _rb;
    private BoxCollider2D _boxCollider;
    private float _speed = 12f;
    private float _lifeTime = 5f;
    private float _life = 0;
    private bool _isHoming;
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _player = GameObject.FindWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        if (_player == null) return;
        if(_life < _lifeTime)
        {
            _life += Time.deltaTime;
            if (_isHoming)
            {            
                Vector2 direction = (_player.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                _rb.rotation = angle;
                _rb.linearVelocity = direction * _speed;
            }
            else
            {

            }
        }
        else
        {
            gameObject.SetActive(false);
            _life = 0;
        }
       
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
        }
    }
}
