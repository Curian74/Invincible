using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private GameObject[] powerupPrefabs;
    [SerializeField] private float dropChance = 0.5f;
    [SerializeField] private float healthBonus = 15f;
    
    private float baseMaxHealth;

    private new void Awake()
    {
         base.Awake();
        baseMaxHealth = maxHealth;
    }
    
    private void OnEnable()
    {
        maxHealth = baseMaxHealth;
        
        int currentCycle = Spawner.GetCurrentCycle();
        if (currentCycle > 0)
        {
            maxHealth += healthBonus * currentCycle;
            Debug.Log($"Applied health bonus of {healthBonus * currentCycle} to enemy. New maxHealth: {maxHealth}");
        }
        
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    protected override void Die()
    {
        TryDropPowerup();
        base.Die();
    }

    private void TryDropPowerup()
    {
        if (powerupPrefabs.Length > 0 && Random.value < dropChance)
        {
            Instantiate(powerupPrefabs[Random.Range(0, powerupPrefabs.Length)], transform.position, Quaternion.identity);
        }
    }
}