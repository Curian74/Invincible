using System.Collections;
using UnityEngine;

public class PoisonDebuff : Powerup
{
    [SerializeField] private float Strength = 5f; // damage per second

    private Health health;
    private Coroutine poisonCoroutine;

    public override void Activate(GameObject target)
    {
        Health health = target.GetComponent<Health>();

        if (health != null)
        {
            target.GetComponent<MonoBehaviour>().StartCoroutine(ApplyPoisonEffect(health));
        }

        Destroy(gameObject);
    }

    private IEnumerator ApplyPoisonEffect(Health health)
    {
        float elapsed = 0f;

        while (elapsed < Duration)
        {
            if (health != null)
            {
                health.TakeDamage(Strength);
                Debug.Log($"Damaged player for: {Strength} HP");
            }

            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }
    }
}
