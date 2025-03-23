using UnityEngine;
using System;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public event Action<float> OnHealthChanged; 
    public event Action OnDeath; 
	[SerializeField] protected float maxHealth = 100f;
    protected float currentHealth;
    private protected BoxCollider2D _collider;
    private protected Rigidbody2D _rb;
    private protected Animator _animator;

    protected void Awake()
    {
        _animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        _collider = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);

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
    }

    protected virtual void Die()
    {
		OnDeath?.Invoke();
        gameObject.SetActive(false);
    }


    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}
