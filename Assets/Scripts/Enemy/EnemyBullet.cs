using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _lifeTime = 3f;
    [SerializeField] private float _damage = 3f;

    private Vector2 _direction;
    private float _spawnTime;

    public void SetDirection(Vector2 direction)
    {
        _direction = direction.normalized;
        _spawnTime = Time.time;
    }

    private void Update()
    {
        transform.position += (Vector3)(_direction * _speed * Time.deltaTime);
        if (Time.time - _spawnTime >= _lifeTime)
            ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                float percentDamage = Random.Range(1f, 3f);
                playerHealth.TakeDamage(_damage + (playerHealth.GetMaxHealth() * percentDamage / 400f));
            }
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (ObjectPool.Instance != null)
            ObjectPool.Instance.ReturnToPool("EnemyBullet", gameObject);
        else
            Destroy(gameObject);
    }
}