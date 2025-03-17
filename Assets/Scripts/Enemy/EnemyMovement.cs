using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _rotationSpeed;

    [SerializeField]
    private float _screenBorder;

    private Rigidbody2D _rigidbody;
    private PlayerAware _playerAwarenessController;
    private Vector2 _targetDirection;
    private Camera _camera;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAwarenessController = GetComponent<PlayerAware>();
        _targetDirection = transform.up;
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        UpdateTargetDirection();
        RotateTowardsTarget();
        SetVelocity();
    }

    private void UpdateTargetDirection()
    {
        HandlePlayerTargeting();
        HandleEnemyOffScreen();
    }

    private void HandlePlayerTargeting()
    {
        if (_playerAwarenessController.AwareOfPlayer)
        {
            _targetDirection = _playerAwarenessController.DirectionToPlayer;
        }
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
        _targetDirection = transform.up;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        enabled = true;
    }

    private void RotateTowardsTarget()
    {
        if (_targetDirection.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
        }
        else if (_targetDirection.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, transform.rotation.eulerAngles.z);
        }
    }

    private void SetVelocity()
    {
        _rigidbody.linearVelocity = _targetDirection.normalized * _speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
            Invoke(nameof(CheckRemainingPlayers), 0.1f);
        }
    }

    private void CheckRemainingPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            DestroyPlayer();
        }
    }

    private void DestroyPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Destroy(player);
            Time.timeScale = 0f;
        }
    }
}