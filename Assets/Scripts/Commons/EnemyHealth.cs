using System;
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private GameObject[] powerupPrefabs; // Array of possible powerups
    [SerializeField] private float dropChance = 0.5f; // chance to drop a powerup

    protected override void Die()
    {
        TryDropPowerup();
        base.Die();
    }

    private void TryDropPowerup()
    {
        if (powerupPrefabs.Length > 0 && UnityEngine.Random.value < dropChance)
        {
            int randomIndex = UnityEngine.Random.Range(0, powerupPrefabs.Length);
            Instantiate(powerupPrefabs[randomIndex], transform.position, Quaternion.identity);
        }
    }
}
