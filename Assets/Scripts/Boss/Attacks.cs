using System.Collections;
using UnityEngine;

public class Attacks : MonoBehaviour
{
	private Animator _animator;
	private float _castCoolDown = 0f;
	private Spell _spell;
	private Transform _player;
	private float _meleeCoolDown = 0f;
	private Movements _movement;
	private bool isTeleporting = false;
	private bool isPerformingAction = false; // Kiểm soát Boss có đang thực hiện hành động nào không

	[SerializeField] private float castCooldownTime = 2f;
	[SerializeField] private float meleeCooldownTime = 2f;

	void Awake()
	{
		_movement = GetComponent<Movements>();
		_player = GameObject.FindWithTag("Player").transform;
		_animator = GetComponent<Animator>();
		_spell = GetComponent<Spell>();
	}

	void Update()
	{
		if (_castCoolDown > 0) _castCoolDown -= Time.deltaTime;
		if (_meleeCoolDown > 0) _meleeCoolDown -= Time.deltaTime;

		float distanceToPlayer = Mathf.Abs(Vector3.Distance(transform.position, _player.position));

		if (!isPerformingAction)
		{
			if (distanceToPlayer <= 8f && distanceToPlayer >= 2f)
			{
				int randomDecision = GetRandomDecision();
				StartCoroutine(PerformStringAttack(randomDecision)); // Sử dụng Coroutine để thực hiện từng hành động một
			}
			else if (distanceToPlayer <= 2f)
			{
				StartCoroutine(PerformMeleeAttack()); // Nếu gần người chơi, thực hiện melee attack
			}
		}
	}

	private IEnumerator PerformStringAttack(int decision)
	{
		isPerformingAction = true;

		switch (decision)
		{
			case 1:
				yield return StartCoroutine(BarageSpell());
				break;
			case 2:
				yield return StartCoroutine(LungeTowardsPlayer());
				break;
			case 3:
				yield return StartCoroutine(TeleportToPlayer());
				break;
		}

		yield return new WaitForSeconds(1f); // Thêm delay trước khi thực hiện hành động tiếp theo
		isPerformingAction = false;
	}

	private IEnumerator BarageSpell()
	{
		for (int i = 0; i < 5; i++)
		{
			CastSpell();
			yield return new WaitForSeconds(0.1f); // Chờ một chút giữa các lần bắn spell
		}
	}

	private void CastSpell()
	{
		if (_castCoolDown <= 0)
		{
			_animator.SetTrigger("CastSpell");
			_castCoolDown = castCooldownTime;
			_spell.SpawnSpell();
		}
	}

	private IEnumerator TeleportToPlayer()
	{
		if (isTeleporting) yield break;
		isTeleporting = true;

		_animator.SetTrigger("Teleport");
		yield return new WaitForSeconds(0.5f);

		float randomX = Random.Range(-1.5f, 1.5f);
		float randomZ = Random.Range(-1.5f, 1.5f);
		Vector3 offset = new Vector3(randomX, 0f, randomZ);

		Vector3 newPosition = _player.position;
		//newPosition = ClampToMapBounds(newPosition);

		transform.position = newPosition;
		_animator.SetTrigger("Appear");
		_animator.SetTrigger("Idle");
		yield return new WaitForSeconds(1f); // Chờ Boss xuất hiện xong rồi mới tiếp tục

		isTeleporting = false;
	}

	private IEnumerator PerformMeleeAttack()
	{
		isPerformingAction = true;
		MeleeAttack();
		yield return new WaitForSeconds(meleeCooldownTime); // Chờ cooldown xong mới tiếp tục
		isPerformingAction = false;
	}

	private void MeleeAttack()
	{
		if (_meleeCoolDown <= 0)
		{
			_animator.SetTrigger("MeleeAttack");
			_meleeCoolDown = meleeCooldownTime;
		}
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
		yield return new WaitForSeconds(0.5f);
	}

	private Vector3 ClampToMapBounds(Vector3 position)
	{
		float minX = -10f, maxX = 10f;
		float minZ = -10f, maxZ = 10f;

		position.x = Mathf.Clamp(position.x, minX, maxX);
		position.z = Mathf.Clamp(position.z, minZ, maxZ);

		return position;
	}

	private int GetRandomDecision()
	{
		return Random.Range(1, 4);
	}
}
