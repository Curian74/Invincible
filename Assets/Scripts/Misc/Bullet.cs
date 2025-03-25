using UnityEngine;

public class Bullet : MonoBehaviour
{
    // [SerializeField] private float speed = 10f;
    // [SerializeField] private float lifeTime = 2f;
    // [SerializeField] private float damage = 5f;
    [SerializeField] private float expGain = 5f;
    // [SerializeField] private float mobScore = 1f;
    [SerializeField] PlayerStats playerStats;
    [SerializeField] private int minPiercingLevel = 5;

    private Vector2 direction;
    private float timer;

    void OnEnable()
    {
        timer = 0f;
    }

    void Update()
    {
        transform.position += playerStats.bulletSpeed * Time.deltaTime * (Vector3)direction;
        timer += Time.deltaTime;
        if (timer >= playerStats.bulletLifeTime)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            Debug.Log("Hit");
            Health health = collision.GetComponent<Health>();

            if (health != null)
            {
                float currentHealth = health.GetCurrentHealth();
                health.TakeDamage(playerStats.damage);

                if (currentHealth > 0 && health.GetCurrentHealth() <= 0)
                {
                    Exp exp = FindFirstObjectByType<Exp>();
                    if (exp != null)
                    {
                        if (collision.gameObject.CompareTag("Boss"))
                        {
                            exp.LevelUp();
                            ScoreManager.Instance.AddScore(10);
                        }
                        else
                        {
                            exp.GainExp(expGain);
                            Debug.Log(exp.GetCurrentExp());
                            ScoreManager.Instance.AddScore(1);
                        }
                    }
                }
            }

            //Xuyen qua neu bulletSpeed >= level 5
            if (playerStats.GetUpgradeCount(UpgradeOption.UpgradeType.BulletSpeed) < minPiercingLevel)
            {
                gameObject.SetActive(false);
            }
        }

        else if (collision.gameObject.CompareTag("Border"))
        {
            gameObject.SetActive(false);
        }
    }


    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }
}
