using System.Collections;
using UnityEngine;

public class Attacks : MonoBehaviour
{
	[SerializeField] private Spell _spellPrefab;
	private Animator _animator;
	private Transform _player;
	private Movements _movement;
	private bool _isTeleporting = false;
	private PoolingManager<Spell> _poolingManager;
	private int _spellPool = 5;
	private bool _isPerformingAction = false;

	void Awake()
	{
		_movement = GetComponent<Movements>();
		_player = GameObject.FindWithTag("Player").transform;
		_animator = GetComponent<Animator>();
		_poolingManager = new PoolingManager<Spell>(_spellPrefab, _spellPool, null);
	}

	void Update()
	{
		float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

		if (!_isPerformingAction)
		{
			if (distanceToPlayer <= 8f && distanceToPlayer >= 2f)
			{
				StartCoroutine(PerformStringAttack(GetRandomDecision()));
			}
			else if (distanceToPlayer < 2f)
			{
				StartCoroutine(PerformMeleeAttack());
			}
		}
	}

	private IEnumerator PerformStringAttack(int decision)
	{
		_isPerformingAction = true;
		switch (decision)
		{
			case 1:
				yield return StartCoroutine(CastSpell());
				break;
			case 2:
				yield return StartCoroutine(LungeTowardsPlayer());
				break;
			case 3:
				yield return StartCoroutine(TeleportToPlayer());
				break;
		}
		if (decision != 2)  yield return new WaitForSeconds(3f); 
		_isPerformingAction = false;
	}

	private IEnumerator LungeTowardsPlayer()
	{
		Vector3 startPosition = transform.position;
		Vector3 targetPosition = _player.position;
		float duration = 0.5f;
		float elapsedTime = 0;

		while (elapsedTime < duration)
		{
			transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		transform.position = targetPosition;
		yield return new WaitForSeconds(0.4f);
	}


	private IEnumerator PerformMeleeAttack()
	{
		_isPerformingAction = true;
		_animator.SetTrigger("MeleeAttack");
		yield return new WaitForSeconds(2f);
		_isPerformingAction = false;
	}

	private IEnumerator CastSpell()
	{
		var spell = _poolingManager.GetObject();
		if (spell != null)
		{
			_animator.SetTrigger("CastSpell");
			spell.SetPosition(); 
			yield return new WaitForSeconds(1f);
			_poolingManager.BackToPool(spell);
		}
	}

	private IEnumerator TeleportToPlayer()
	{
		if (_isTeleporting) yield break;
		_isTeleporting = true;

		_animator.SetTrigger("Teleport");
		yield return new WaitForSeconds(0.5f);

		Vector3 newPosition = _player.position + new Vector3(Random.Range(-1.5f, 1.5f), 0, Random.Range(-1.5f, 1.5f));
		transform.position = newPosition;

		_animator.SetTrigger("Appear");
		yield return new WaitForSeconds(0.5f);
		QuickSlash();
		yield return new WaitForSeconds(2f); 
		_isTeleporting = false;
	}

	private void QuickSlash()
	{
		_animator.SetTrigger("QuickSlash");
		_animator.SetTrigger("Idle");
	}

	private int GetRandomDecision()
	{
		return Random.Range(1, 4);
	}
}
