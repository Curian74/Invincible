using UnityEngine;

public class HealthPowerup : Powerup
{
    [SerializeField] private float Strength = 15f;

    public override void Activate(GameObject target)
    {
        base.Activate(target);
    }

    public override void ApplyEffect(GameObject Target)
    {
        Health health = Target.GetComponent<Health>();
        if (health != null)
        {
            health.Heal(Strength);
        }
    }
}
