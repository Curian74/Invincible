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
	public float damageMultiplier = 1f;
	private bool _boxColliderEnabled;
    private bool hasHit = false;
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
        hasHit = false;
        gameObject.SetActive(true);
		_animator.SetTrigger("SpawnSpell");
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!hasHit && other.CompareTag("Player"))
		{
			_playerHealth = other.GetComponent<Health>();
			if (_playerHealth != null)
			{
				_playerHealth.TakeDamage(damage * damageMultiplier);
			}
		}
        hasHit = true;

    }

    private void OnEnable()
    {
        damageMultiplier += 0.11f;
    }
}



