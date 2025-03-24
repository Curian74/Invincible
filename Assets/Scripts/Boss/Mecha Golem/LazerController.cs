using UnityEngine;

public class LazerController : MonoBehaviour
{
    float rotationSpeed = 65f;
    float maxAngle = 360f; 
    float offsetAbovePlayer = 1f;
    [SerializeField] int delayFrames = 35; 

    private float _length;
    private Transform _player;
    private float _currentAngle;
    private Vector3 _pivotPoint;
    private int _frameCounter;
    private bool _isFlipped;

    void Awake()
    {
        _length = GetComponent<SpriteRenderer>().bounds.extents.y;
        _player = GameObject.FindWithTag("Player")?.transform;

        if (_player == null)
        {
            Debug.LogError("No player found!");
        }
    }

    void OnEnable()
    {
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
}
