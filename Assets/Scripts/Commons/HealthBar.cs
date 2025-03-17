using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Health health;
    private Slider slider;

    void Start()
    {
        health = FindFirstObjectByType<Health>();
        slider = GetComponent<Slider>();
        slider.value = health.GetCurrentHealth() / health.GetMaxHealth();
    }

    public void UpdateHealthBar(float healthPercent)
    {
        slider.value = healthPercent;
    }
}
