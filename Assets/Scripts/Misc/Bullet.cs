using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;
    // [SerializeField] private float damage = 5f;
    [SerializeField] private float expGain = 5f;
    // [SerializeField] private float mobScore = 1f;
    [SerializeField] PlayerStats playerStats;

    private Vector2 direction;
    private float timer;

    void OnEnable()
    {
        timer = 0f;
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime * (Vector3)direction;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Border"))
        {
            Debug.Log("Hit");
            Health health = collision.GetComponent<Health>();

            if (health != null)
            {
                float currentHealth = health.GetCurrentHealth();
                health.TakeDamage(playerStats.damage);

                // Only grant EXP and Score if this bullet landed the killing blow
                if (currentHealth > 0 && health.GetCurrentHealth() <= 0)
                {
                    Exp exp = FindFirstObjectByType<Exp>();
                    if (exp != null)
                    {
                        exp.GainExp(expGain);
                        Debug.Log(exp.GetCurrentExp());
                    }
                    ScoreManager.Instance.AddScore(1);
                }
            }

            gameObject.SetActive(false);
        }
    }


    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }
}
