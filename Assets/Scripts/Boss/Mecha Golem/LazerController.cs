using UnityEngine;

public class LazerController : MonoBehaviour
{
    [SerializeField] int delayFrames = 35;
    float rotationSpeed = 65f;
    float maxAngle = 360f; 
    float offsetAbovePlayer = 1f;
    private float _length;
    private Transform _player;
    private float _currentAngle;
    private Vector3 _pivotPoint;
    private int _frameCounter;
    private bool _isFlipped;
    private bool _hasHit = false;
    private float _damage = 35;
    private Rigidbody2D _rb;
    private BoxCollider2D _boxCollider;
    public float _damageMultiplier = 1;
    void Awake()
    {
        _length = GetComponent<SpriteRenderer>().bounds.extents.y;
        _player = GameObject.FindWithTag("Player")?.transform;
        _rb = GetComponent<Rigidbody2D>();
        if (_player == null)
        {
            Debug.LogError("No player found!");
        }
    }

    void OnEnable()
    {
       // _hasHit = false;
        if (_player != null)
        {
            if (transform.parent?.parent != null)
            {
                _isFlipped = _player.position.x > transform.position.x;
                transform.parent.parent.localScale = new Vector3(_isFlipped ? 8 : -8, 8, 1);
            }

            _pivotPoint = _player.position + Vector3.up * offsetAbovePlayer;
            _frameCounter = 0;

            Vector3 directionToLaser = (transform.position - _pivotPoint).normalized;
            float initialAngle = Mathf.Atan2(directionToLaser.y, directionToLaser.x) * Mathf.Rad2Deg;

            _currentAngle = _isFlipped ? initialAngle -120 : initialAngle + 50f;
            _currentAngle = Mathf.Clamp(_currentAngle, -maxAngle, maxAngle);
        }
    }

    void Update()
    {
        if (_player == null) return;

        if (_frameCounter < delayFrames)
        {
            _frameCounter++;
            return;
        }

        if (_currentAngle > -maxAngle)
        {
            _currentAngle -= rotationSpeed * Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }

        Quaternion rotation = Quaternion.Euler(0, 0, _currentAngle);
        transform.position = _pivotPoint + rotation * (Vector3.up * _length);
        transform.rotation = rotation;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit == false && other.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_damage * _damageMultiplier);
            }
        }
        _hasHit = false;
    }
}
