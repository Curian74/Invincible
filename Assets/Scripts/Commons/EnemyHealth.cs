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
        if (powerupPrefabs.Length > 0 && Random.value < dropChance)
        {
            int randomIndex = Random.Range(0, powerupPrefabs.Length);
            Instantiate(powerupPrefabs[randomIndex], transform.position, Quaternion.identity);
        }
    }
}
