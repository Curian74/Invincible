using UnityEngine;

public class SpeedPowerup : Powerup
{
    [SerializeField] private float Strength = 1.5f; // 2 = x2 speed

    public override void Activate(GameObject target)
    {
        base.Activate(target);
    }

    public override void ApplyEffect(GameObject target)
    {
        PlayerStats stats = GameObject.FindGameObjectWithTag("PlayerStats").GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.speed *= Strength;
        }
    }

    public override void RemoveEffect(GameObject target)
    {
        PlayerStats stats = GameObject.FindGameObjectWithTag("PlayerStats").GetComponent<PlayerStats>();

        if (stats != null)
        {
            stats.speed /= Strength;
        }
    }
}
