using Assets.Scripts.Powerups;
using System.Collections;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] internal float Duration; // how long the effect lasts
    [SerializeField] internal float Lifetime = 15; // time the pickup stays active
    
    private float _timeActive = 0;

    void Update()
    {
        _timeActive += Time.deltaTime;
        if (_timeActive >= Lifetime)
        {
            Destroy(gameObject);
        }
    }

    public virtual void Activate(GameObject target)
    {
        PowerupManager manager = target.GetComponent<PowerupManager>();
        if (manager != null)
        {
            manager.ApplyPowerup(this, target);
        }

        Destroy(gameObject);
    }


    public virtual void ApplyEffect(GameObject target)
    {
    }

    public virtual void RemoveEffect(GameObject target)
    {
    }

    private IEnumerator DeactivateAfterTime(GameObject target)
    {
        yield return new WaitForSeconds(Duration);
        RemoveEffect(target);
    }
}
