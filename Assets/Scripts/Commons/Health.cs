using UnityEngine;
using System;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public event Action<float> OnHealthChanged; // Event for UI updates
    public event Action OnDeath; // Event when entity dies

    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        Debug.Log($"Current health: {currentHealth}");
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        Debug.Log($"Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        //Heal khi an pack hp (khong the vuot qua maxHealth)
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        Debug.Log($"Current health: {currentHealth}");
    }

    private void Die()
    {
        //Co the them animation o day
        HealthBar healthBar = GetComponentInChildren<HealthBar>();
        ExpBar expBar = GetComponentInChildren<ExpBar>();

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        if(expBar != null)
        {
            expBar.gameObject.SetActive(false);
        }

        OnDeath?.Invoke();
        gameObject.SetActive(false);
    }


    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}
