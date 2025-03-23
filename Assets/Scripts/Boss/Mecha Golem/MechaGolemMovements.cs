using UnityEngine;

public class MechaGolemMovements : MonoBehaviour
{
    [SerializeField] private float moveRadius = 7f;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float minDistance = 3f;
    private Rigidbody2D _rb;
    private Animator _animator;
    private Transform _player;
    private Vector2 targetPosition;
    private SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _player = GameObject.FindWithTag("Player").transform;
        _spriteRenderer = GetComponent<SpriteRenderer>(); // Get sprite renderer
    }

    void Start()
    {
        InvokeRepeating(nameof(SetNewTargetPosition), 0f, 3f);
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        if(IsPlayingAnimation("Laser") || IsPlayingAnimation("Defend"))
        {
            return;
        }
        if (distanceToPlayer <= minDistance)
        {
            MoveAway();
        }
        MoveToTarget();
    }

    private void SetNewTargetPosition()
    {
        if (_player == null) return;
        Vector2 randomDirection = Random.insideUnitCircle.normalized * moveRadius;
        targetPosition = (Vector2)_player.position + randomDirection;
    }

    private void MoveAway()
    {
        Vector2 directionAway = (transform.position - _player.position).normalized;
        targetPosition = (Vector2)transform.position + directionAway * moveRadius;
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        if ((Vector2)transform.position != targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            FlipSprite(targetPosition);
        }
    }

    public void FlipSprite(Vector2 target)
    {
        if (target.x > transform.position.x)
            transform.localScale = new Vector3(6, 6, 1);  
        else if (target.x < transform.position.x)
            transform.localScale = new Vector3(-6, 6, 1); 
    }

    private bool IsPlayingAnimation(string animationName)
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime < 1.0f;
    }
}
