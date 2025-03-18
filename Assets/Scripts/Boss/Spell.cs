using System;
using System.Collections;
using UnityEngine;

public class Spell : MonoBehaviour
{
	private Transform _player;
	private Animator _animator;
	private BoxCollider2D _boxCollider;
	private Vector3 _offset = new Vector3(0, 2f, 0);
    void Awake()
    {
        _animator = GetComponent<Animator>();
		_boxCollider = GetComponent<BoxCollider2D>();
		_player = GameObject.FindWithTag("Player").transform;
    }

	public void SetPosition()
	{
		transform.position = _player.transform.position + _offset;
		transform.localScale = new Vector3(4, 5, 1);
		gameObject.SetActive(true);
		_animator.SetTrigger("SpawnSpell");
	}


}
