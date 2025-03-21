using System;
using Unity.Mathematics;
using UnityEngine;

public class Movements : MonoBehaviour
{
	[SerializeField] private float _detectionRange = 100f;
	[SerializeField] private LayerMask _playerLayer;
	public float _movementSpeed = 5f;
	private bool _playerDetected = false;
	private Animator _animator;
	private Transform _player;
	private Attacks _attacks;
	private GameObject _bossTopBar;


	void Awake()
	{
		_player = GameObject.FindWithTag("Player").transform;
		_animator = GetComponent<Animator>();
		_attacks = GetComponent<Attacks>();
		_bossTopBar = GameObject.FindGameObjectWithTag("BossTopBar");
	}

	void Update()
	{
		DetectPlayer();
		_animator.SetBool("PlayerDetected", _playerDetected);
		if (IsPlayingAnimation("Appear"))
		{
			return;
		}
		if (_playerDetected)
		{
			MoveTowardsPlayer();
		}
		float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
		if (distanceToPlayer <= 15f)
		{
			_bossTopBar.SetActive(true);
		}
		else
		{
			_bossTopBar.SetActive(false);
		}
	}

	private void DetectPlayer()
	{
		if (_player.gameObject.activeInHierarchy)
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
		else
		{
			_playerDetected = false;	
		}
	}

	private void MoveTowardsPlayer()
	{
		if (_player != null && _player.gameObject.activeInHierarchy)
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
