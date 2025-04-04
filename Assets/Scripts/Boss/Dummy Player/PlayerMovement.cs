using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] public float speed = 5;
	private Rigidbody2D rb;
	private float inputX;
	private float inputY;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		inputX = Input.GetAxis("Horizontal");
        if (Input.GetMouseButtonDown(0))
        {
            GameObject.FindWithTag("Boss").GetComponent<Health>().TakeDamage(10);
        }
        inputY = Input.GetAxis("Vertical");
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
