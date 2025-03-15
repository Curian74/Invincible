using System;
using UnityEngine;

public class Movements : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _detectionRange = 10f;
	[SerializeField] private LayerMask _playerLayer;
	[SerializeField] private float _orbitSpeed = 1f;
	[SerializeField] private float _orbitRadius = 3f; 
	private bool _isOrbiting = false;
	private float _orbitAngle = 0f;
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
		float distanceToPlayer = Mathf.Abs(Vector3.Distance(transform.position, _player.position));

		if (_isOrbiting)
		{
			OrbitAroundPlayer();
		}

		if (distanceToPlayer >= 10f) 
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
		else if(distanceToPlayer >= 8f)
		{
			_attacks.CastSpell();
			Invoke(nameof(LungeTowardsPlayer), 0.8f);
			//LungeTowardsPlayer();				
			//_isOrbiting = true;
		}
		else if (distanceToPlayer >= 2f || distanceToPlayer <= 2f)
		{
			_attacks.MelleAttack();
		}
		
	}

	public void LungeTowardsPlayer()
	{	
		Vector3 direction = (_player.position - transform.position).normalized;
		for (int i = 0; i < 2; i++)
		{
			transform.position += direction * _movementSpeed * Time.deltaTime * 4;
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
	public void OrbitAroundPlayer()
	{
		_orbitAngle += _orbitSpeed * Time.deltaTime; 
		float x = _player.position.x + Mathf.Cos(_orbitAngle) * _orbitRadius;
		float y = _player.position.y + Mathf.Sin(_orbitAngle) * _orbitRadius;
		transform.position = new Vector3(x, y, transform.position.z);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _detectionRange);
	}
}
