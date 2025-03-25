using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class AttackController : MonoBehaviour
{
	[SerializeField] private float _attackDamage = 30f;
	[SerializeField] public float damageMultiplier = 1f;
	private string _playerTag = "Player";
	private Transform _player;
	private PolygonCollider2D _collider;
	private Health _playerHealth;



	private void Awake()
	{
		_collider = GetComponent<PolygonCollider2D>();
		_player = GameObject.FindWithTag("Player").transform;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag(_playerTag))
		{
			float overallDamage = _attackDamage * damageMultiplier;
			_playerHealth = other.GetComponent<Health>();
			if (_playerHealth != null)
			{
				_playerHealth.TakeDamage(overallDamage);
			}
		}
	}

    private void OnEnable()
    {
        damageMultiplier += 0.11f;
    }

}
