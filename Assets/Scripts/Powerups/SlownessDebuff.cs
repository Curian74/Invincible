using UnityEngine;

public class SlownessDebuff : Powerup
{
    [SerializeField] private float Strength = 1.5f; // 2 = 1/2 speed

    public override void Activate(GameObject target)
    {
        base.Activate(target);
    }

    public override void ApplyEffect(GameObject target)
    {
        PlayerStats movement = target.GetComponent<PlayerStats>();

        if (movement != null)
        {
            movement.speed /= Strength;
        }
    }

    public override void RemoveEffect(GameObject target)
    {
        PlayerStats movement = target.GetComponent<PlayerStats>();

        if (movement != null)
        {
            movement.speed *= Strength;
        }
    }
}
