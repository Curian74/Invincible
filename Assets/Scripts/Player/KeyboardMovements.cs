using UnityEngine;

public class KeyboardMovements : MonoBehaviour
{
    [SerializeField] public float speed = 5;
    private Rigidbody2D rb;
    private Animator anim;
    private float inputX;
    private float inputY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        Vector2 moveDirection = new(inputX, inputY);

        if (moveDirection.magnitude > 0)
        {
            anim.SetBool("run", true);
            moveDirection.Normalize(); // Ensures uniform speed in all directions
        }
        else
        {
            anim.SetBool("run", false);
        }

        rb.linearVelocity = moveDirection * speed;
    }

    //Testing function
    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Enemy"))
    //     {
    //         Health health = GetComponent<Health>();
    //         if (health != null)
    //         {
    //             health.TakeDamage(15);
    //             HealthBar healthBar = FindFirstObjectByType<HealthBar>();
    //             if (healthBar != null)
    //             {
    //                 healthBar.UpdateHealthBar(health.GetCurrentHealth() / health.GetMaxHealth());
    //             }
    //         }
    //     }
    // }
}
