using UnityEngine;
using UnityEngine.Events;

public class HealthControllerEnemy : MonoBehaviour
{
    [SerializeField]
    private float _currentHealth;

    [SerializeField]
    private float _maximumHealth;

    public float RemainingHealthPercentage => _currentHealth / _maximumHealth;

    public bool IsInvincible { get; set; }

    public UnityEvent OnDied;
    public UnityEvent OnDamaged;
    public UnityEvent OnHealthChanged;

    private void OnEnable() 
    {
        _currentHealth = _maximumHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        if (_currentHealth <= 0 || IsInvincible)
        {
            return;
        }

        _currentHealth -= damageAmount;

        OnHealthChanged?.Invoke(); 

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            OnDied?.Invoke();
        }
        else
        {
            OnDamaged?.Invoke(); 
        }
    }

    public void AddHealth(float amountToAdd)
    {
        if (_currentHealth >= _maximumHealth)
        {
            return;
        }

        _currentHealth += amountToAdd;

        OnHealthChanged?.Invoke(); 

        if (_currentHealth > _maximumHealth)
        {
            _currentHealth = _maximumHealth;
        }
    }
}
