using UnityEngine;

public class MechaGolemMovements : MonoBehaviour
{
    [SerializeField] private float moveRadius = 4f;
    [SerializeField] private float moveSpeed = 10f; // Thêm tốc độ di chuyển
    private Rigidbody2D _rb;
    private Animator _animator;
    private Transform _player;
    private Vector2 targetPosition;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _player = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {
        InvokeRepeating(nameof(SetNewTargetPosition), 0f, 3f); // Gọi mỗi 3 giây để đổi mục tiêu
    }

    void Update()
    {
        MoveToTarget();
    }

    private void SetNewTargetPosition()
    {
        if (_player == null) return;

        // Chọn một vị trí ngẫu nhiên xung quanh người chơi
        Vector2 randomDirection = Random.insideUnitCircle.normalized * moveRadius;
        targetPosition = (Vector2)_player.position + randomDirection;
    }

    private void MoveToTarget()
    {
        if ((Vector2)transform.position != targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
}
