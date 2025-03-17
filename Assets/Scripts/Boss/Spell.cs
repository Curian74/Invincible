using System.Collections;
using UnityEngine;

public class Spell : MonoBehaviour
{
	[SerializeField] private Transform _player;
	private Animator _animator;
	private BoxCollider2D _boxCollider;
	[SerializeField] private GameObject _spell;
	//private PoolingManager<Spell> _poolingManager;
    void Awake()
    {
        _animator = GetComponent<Animator>();
		_boxCollider = GetComponent<BoxCollider2D>();
		_player = GameObject.FindWithTag("Player").transform;
		//_poolingManager = GetComponent<PoolingManager<Spell>>();
    }

	public void SpawnSpell()
	{
		_spell.transform.position = _player.transform.position;
	    _spell.gameObject.SetActive(true);
		_animator.SetTrigger("SpawnSpell");
	}

	private void SetPosition(Transform player)
	{
		//transform.position = player.position + Vector3.up * 5;
		transform.position = player.position;
	}
}
