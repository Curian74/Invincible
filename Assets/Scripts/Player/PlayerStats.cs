using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float damage = 10f;
    public float speed = 5f;
    public float fireRate = 0.5f;
    public float bulletLifeTime = 0.5f;
    public float bulletSpeed = 10f;
    private Dictionary<UpgradeOption.UpgradeType, int> upgradeCounts = new Dictionary<UpgradeOption.UpgradeType, int>();

    void Start()
    {
        foreach (UpgradeOption.UpgradeType type in System.Enum.GetValues(typeof(UpgradeOption.UpgradeType)))
        {
            upgradeCounts[type] = 0;
        }
    }

    public void IncreaseHealth(float amount)
    {
        PlayerHealth health = FindFirstObjectByType<PlayerHealth>();
        if (health != null)
        {
            health.IncreaseHealth(amount);
            health.UpdateHealthUI();
        }
        IncrementUpgradeCount(UpgradeOption.UpgradeType.Health);
    }

    public void IncreaseDamage(float amount)
    {
        damage += amount;
        Debug.Log($"Damage: {damage}");
        IncrementUpgradeCount(UpgradeOption.UpgradeType.Damage);
    }

    public void IncreaseSpeed(float amount)
    {
        speed += amount;
        Debug.Log($"Movement Speed: {speed}");
        IncrementUpgradeCount(UpgradeOption.UpgradeType.Speed);
    }

    // public void IncreaseFireRate(float amount)
    // {
    //     fireRate -= amount;
    //     if (fireRate < 0.1f) fireRate = 0.1f;
    //     Debug.Log($"FireRate reduced: {fireRate}");
    // }

    public void IncreaseBulletLifeTime(float amount)
    {
        bulletLifeTime += amount;
        Debug.Log($"Bullet Life Time: {bulletLifeTime}");
        IncrementUpgradeCount(UpgradeOption.UpgradeType.BulletLifeTime);
    }

    public void IncreaseBulletSpeed(float amount)
    {
        bulletSpeed += amount;
        Debug.Log($"Bullet Speed: {bulletSpeed}");
        IncrementUpgradeCount(UpgradeOption.UpgradeType.BulletSpeed);
    }

    private void IncrementUpgradeCount(UpgradeOption.UpgradeType type)
    {
        if (upgradeCounts.ContainsKey(type))
        {
            upgradeCounts[type]++;
        }
    }

    public int GetUpgradeCount(UpgradeOption.UpgradeType type)
    {
        return upgradeCounts.ContainsKey(type) ? upgradeCounts[type] : 0;
    }
}
