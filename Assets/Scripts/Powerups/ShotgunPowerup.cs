using UnityEngine;

public class ShotgunPowerup : WeaponPowerup
{
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
            currentFirerate = playerStats.fireRate;
            fireRate = playerStats.fireRate * fireRateMultiplier;
            playerStats.fireRate = fireRate;
            shooting.MultiShot = true;
        }
    }

    public override void RemoveEffect(GameObject target)
    {
        Shooting shooting = target.GetComponent<Shooting>();
        PlayerStats playerStats = shooting.playerStats;

        if (shooting != null)
        {
            playerStats.fireRate = fireRate / fireRateMultiplier;
            shooting.MultiShot = false;
        }
    }
}
