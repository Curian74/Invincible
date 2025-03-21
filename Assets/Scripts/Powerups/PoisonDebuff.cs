using Assets.Scripts.Powerups;
using System.Collections;
using UnityEngine;

public class PoisonDebuff : Powerup
{
    [SerializeField] private float Strength = 5f; // damage per second

    public override void Activate(GameObject target)
    {
        PowerupManager manager = target.GetComponent<PowerupManager>();
        if (manager != null)
        {
            manager.ApplyPowerup(this, target);
        }

        Destroy(gameObject);
    }

    public override void ApplyEffect(GameObject target)
    {
        Health health = target.GetComponent<Health>();
        if (health != null)
        {
            target.GetComponent<MonoBehaviour>().StartCoroutine(ApplyPoisonEffect(health));
        }
    }

    private IEnumerator ApplyPoisonEffect(Health health)
    {
        float elapsed = 0f;

        while (elapsed < Duration)
        {
            if (health != null)
            {
                health.TakeDamage(Strength);
            }

            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }
    }
}
