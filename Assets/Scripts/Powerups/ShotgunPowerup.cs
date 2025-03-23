using UnityEngine;

public class ShotgunPowerup : WeaponPowerup
{
    [SerializeField] public float Cooldown = 0.6f;

    public override void Activate(GameObject target)
    {
        base.Activate(target);
    }

    public override void ApplyEffect(GameObject target)
    {
        Shooting shooting = target.GetComponent<Shooting>();

        if (shooting != null)
        {
            currentCooldown = shooting.coolDown;
            shooting.coolDown = Cooldown;
            shooting.MultiShot = true;
        }
    }

    public override void RemoveEffect(GameObject target)
    {
        Shooting shooting = target.GetComponent<Shooting>();

        if (shooting != null)
        {
            shooting.coolDown = currentCooldown;
            shooting.MultiShot = false;
        }
    }
}
