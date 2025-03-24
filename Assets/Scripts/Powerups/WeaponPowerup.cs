using UnityEngine;
using UnityEngine.Rendering;

public class WeaponPowerup : Powerup
{
    protected float currentFirerate = 0.3f;
    [SerializeField] protected float fireRateMultiplier = 2;
    protected float fireRate;
}
