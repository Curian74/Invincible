using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] public float speed = 5;
	private Rigidbody2D rb;
	private float inputX;
	private float inputY;
	private Transform _boss;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		_boss = GameObject.FindWithTag("BringerOfDeath").transform;
	}

	void Update()
	{
		inputX = Input.GetAxis("Horizontal");
		inputY = Input.GetAxis("Vertical");
		if (Input.GetMouseButtonDown(0)) 
		{
			var bossHealth = _boss.GetComponent<Health>();
			bossHealth.TakeDamage(10);
		}

		rb.linearVelocity = new Vector2(inputX * speed, inputY * speed);
	}

	//Testing function
	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Enemy"))
		{
			Health health = GetComponent<Health>();
			if (health != null)
			{
				HealthBar healthBar = FindFirstObjectByType<HealthBar>();
				if (healthBar != null)
				{
					healthBar.UpdateHealthBar(health.GetCurrentHealth() / health.GetMaxHealth());
				}
			}
		}
	}
}
