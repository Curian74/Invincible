using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Powerups
{
    public class PowerupManager : MonoBehaviour
    {
        private readonly Dictionary<Type, (Coroutine, Powerup)> activePowerups = new();

        public void ApplyPowerup(Powerup powerup, GameObject target)
        {
            Type powerupType = powerup.GetType();

            if (powerup is WeaponPowerup)
            {
                Type existingWeaponPowerupType = activePowerups.Keys.FirstOrDefault(t => typeof(WeaponPowerup).IsAssignableFrom(t));
                if (existingWeaponPowerupType != null)
                {
                    var (existingCoroutine, existingPowerup) = activePowerups[existingWeaponPowerupType];
                    StopCoroutine(existingCoroutine);
                    existingPowerup.RemoveEffect(target);
                    activePowerups.Remove(existingWeaponPowerupType);
                }
            }

            if (activePowerups.ContainsKey(powerupType))
            {
                var (existingCoroutine, existingPowerup) = activePowerups[powerupType];
                StopCoroutine(existingCoroutine);
                existingPowerup.RemoveEffect(target);
            }

            Coroutine newCoroutine = StartCoroutine(PowerupEffectCoroutine(powerup, target));
            activePowerups[powerupType] = (newCoroutine, powerup);
        }

        private IEnumerator PowerupEffectCoroutine(Powerup powerup, GameObject target)
        {
            Type powerupType = powerup.GetType();

            powerup.ApplyEffect(target);
            Debug.Log($"Effect applied: {powerupType.Name}");

            yield return new WaitForSeconds(powerup.Duration);

            if (activePowerups.ContainsKey(powerupType))
            {
                powerup.RemoveEffect(target);
                activePowerups.Remove(powerupType);
            }
        }
    }
}
