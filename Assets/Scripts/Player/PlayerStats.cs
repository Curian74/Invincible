using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float damage = 10f;
    public float speed = 5f;
    public float fireRate = 0.5f;
    public float bulletLifeTime = 0.5f;
    public float bulletSpeed = 10f;

    public void IncreaseHealth(float amount)
    {
        PlayerHealth health = FindFirstObjectByType<PlayerHealth>();
        if (health != null)
        {
            health.IncreaseHealth(amount);
            health.UpdateHealthUI();
        }
    }

    public void IncreaseDamage(float amount)
    {
        damage += amount;
        Debug.Log($"Damage: {damage}");
    }

    public void IncreaseSpeed(float amount)
    {
        speed += amount;
        Debug.Log($"Movement Speed: {speed}");
    }

    public void IncreaseFireRate(float amount)
    {
        fireRate -= amount;
        if (fireRate < 0.1f) fireRate = 0.1f;
        Debug.Log($"FireRate reduced: {fireRate}");
    }

    public void IncreaseBulletLifeTime(float amount)
    {
        bulletLifeTime += amount;
        Debug.Log($"Bullet Life Time: {bulletLifeTime}");
    }

    public void IncreaseBulletSpeed(float amount)
    {
        bulletSpeed += amount;
        Debug.Log($"Bullet Speed: {bulletSpeed}");
    }
}
