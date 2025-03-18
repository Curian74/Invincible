using UnityEngine;

public class SpeedPowerup : Powerup
{
    [SerializeField] private float Strength = 1.5f; // 2 = x2 speed

    public override void Activate(GameObject target)
    {
        base.Activate(target);
    }

    protected override void ApplyEffect(GameObject Target)
    {
        KeyboardMovements movement = Target.GetComponent<KeyboardMovements>();

        if (movement != null)
        {
            movement.speed *= Strength;
        }
    }

    protected override void RemoveEffect(GameObject target)
    {
        KeyboardMovements movement = Target.GetComponent<KeyboardMovements>();

        if (movement != null)
        {
            movement.speed /= Strength;
        }
    }
}
