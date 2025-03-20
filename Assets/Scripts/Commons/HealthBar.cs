using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Health health;
    private Slider slider;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        health = player.GetComponent<Health>();
        slider = GetComponent<Slider>();
        slider.value = health.GetCurrentHealth() / health.GetMaxHealth();
        health.OnHealthChanged += UpdateHealthBar;
    }

    public void UpdateHealthBar(float healthPercent)
    {
        slider.value = healthPercent;
    }
}
