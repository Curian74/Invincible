using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Health health;
    private Slider slider;

    void Start()
    {
		Transform parentTransform = transform.parent?.parent;
		health = parentTransform.GetComponent<Health>();
		if (parentTransform != null)
		{
			health = parentTransform.GetComponent<Health>();
		}
		if (health != null)
		{
			slider = GetComponent<Slider>();
			slider.value = health.GetCurrentHealth() / health.GetMaxHealth();
			health.OnHealthChanged += UpdateHealthBar;
		}
		else
		{
			Debug.LogError("Boss Health component not found!");
		}
	}

    public void UpdateHealthBar(float healthPercent)
    {
        slider.value = healthPercent;
    }
}
