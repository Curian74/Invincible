using UnityEngine;

public class HealthPowerup : Powerup
{
    [SerializeField] private float RecoveryAmount = 15f;

    public override void Activate(GameObject target)
    {
        base.Activate(target);
    }

    protected override void ApplyEffect(GameObject Target)
    {
        Health health = Target.GetComponent<Health>();
        if (health != null)
        {
            health.Heal(RecoveryAmount);
        }
    }
}
