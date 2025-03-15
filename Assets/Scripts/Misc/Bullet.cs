using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;

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
        if(collision.CompareTag("Enemy"))
        {
            collision.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }
}
