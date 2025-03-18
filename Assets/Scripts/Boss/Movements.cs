using System;
using Unity.Mathematics;
using UnityEngine;

public class Movements : MonoBehaviour
{
    [SerializeField] private float _detectionRange = 10f;
	[SerializeField] private LayerMask _playerLayer;
	public float _movementSpeed = 5f;
	private bool _playerDetected = false;
	private Animator _animator;
	private Transform _player;
	private Attacks _attacks;
	public bool isPerformingAction = false;

	void Awake()
    {
		_player = GameObject.FindWithTag("Player").transform;
		_animator = GetComponent<Animator>();
		_attacks = GetComponent<Attacks>();
	}

    void Update()
    {
        DetectPlayer();
		_animator.SetBool("PlayerDetected", _playerDetected);
		if (IsPlayingAnimation("Appear"))
		{
			return;
		}
		if (_playerDetected && !isPerformingAction)
		{
			MoveTowardsPlayer();					
		}
		
	}

    private void DetectPlayer()
    {
		Collider2D hit = Physics2D.OverlapCircle(transform.position, _detectionRange, _playerLayer);
		if (hit != null)
		{		
			_playerDetected = true;
		}
		else
		{
			_playerDetected = false;
		}
	}

	private void MoveTowardsPlayer()
	{
			Vector3 direction = (_player.position - transform.position).normalized;
			transform.position += direction * _movementSpeed * Time.deltaTime;

			if (direction.x < 0)
			{
				transform.localScale = new Vector3(5, 5, 5);
			}
			else if (direction.x > 0)
			{
				transform.localScale = new Vector3(-5, 5, 5);
			}					
	}

	private bool IsPlayingAnimation(string animationName)
	{
		AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
		return stateInfo.IsName(animationName) && stateInfo.normalizedTime < 1.0f;
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _detectionRange);
	}
}
