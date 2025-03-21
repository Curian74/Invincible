using System;
using System.Collections;
using UnityEngine;

public class Spell : MonoBehaviour
{
	private Transform _player;
	private Health _playerHealth;
	private Animator _animator;
	private BoxCollider2D _boxCollider;
	private Vector3 _offset = new Vector3(0, 2f, 0);
	private float damage = 30;
	private bool _boxColliderEnabled;
	void Awake()
	{
		_animator = GetComponent<Animator>();
		_boxCollider = GetComponent<BoxCollider2D>();
		_player = GameObject.FindWithTag("Player").transform;
	}

	public void SetPosition()
	{
		transform.position = _player.transform.position + _offset;
		transform.localScale = new Vector3(4, 6, 1);
		_boxCollider.enabled = true;
		gameObject.SetActive(true);
		_animator.SetTrigger("SpawnSpell");
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			_playerHealth = other.GetComponent<Health>();
			if (_playerHealth != null)
			{
				_playerHealth.TakeDamage(damage);
			}
		}
	}
}



