using System;
using UnityEngine;

public class Movements : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _detectionRange = 7f;
	[SerializeField] private LayerMask _playerLayer;
	private bool _playerDetected = false;
	private Animator _animator;
	private Transform _player;
	private Attacks _attacks;
	void Awake()
    {
		_player = GameObject.FindWithTag("Player").transform;
		_animator = GetComponent<Animator>();
		_attacks = GetComponent<Attacks>();
	}

    void Update()
    {
        DetectPlayer();
		if (_playerDetected)
		{
			_animator.SetBool("PlayerDetected", _playerDetected);
			MoveTowardsPlayer();
			Debug.Log(_playerDetected);
			Console.WriteLine(_playerDetected);
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
		float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

		if (distanceToPlayer > 3f) 
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
		else 
		{
			_attacks.CastSpell();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _detectionRange);
	}
}
