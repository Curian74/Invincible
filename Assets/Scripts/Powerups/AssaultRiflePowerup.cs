using UnityEngine;
using UnityEngine.Rendering;

public class AssaultRiflePowerup : WeaponPowerup
{
    [SerializeField] public float Cooldown = 0.1f;

    public override void Activate(GameObject target)
    {
        base.Activate(target);
    }

    public override void ApplyEffect(GameObject target)
    {
        Shooting shooting = target.GetComponent<Shooting>();
        PlayerStats playerStats = shooting.playerStats;

        if (shooting != null)
        {
            fireRate = playerStats.fireRate / fireRateMultiplier;
            playerStats.fireRate = fireRate;
            shooting.MultiShot = false;
        }
    }

    public override void RemoveEffect(GameObject target)
    {
        Shooting shooting = target.GetComponent<Shooting>();
        PlayerStats playerStats = shooting.playerStats;

        if (shooting != null)
        {
            playerStats.fireRate = fireRate * fireRateMultiplier;
        }
    }
}
