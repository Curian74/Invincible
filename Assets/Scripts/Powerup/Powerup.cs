using System.Collections;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] protected GameObject Target;
    [SerializeField] protected float Duration; // how long the effect lasts
    [SerializeField] protected float Lifetime = 15; // time the pickup stays active
    
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
        ApplyEffect(target);

        if (Duration > 0)
        {
            StartCoroutine(DeactivateAfterTime(target));
        }

        Destroy(gameObject);
    }


    protected virtual void ApplyEffect(GameObject target)
    { 
    }

    protected virtual void RemoveEffect(GameObject target)
    {
    }

    private IEnumerator DeactivateAfterTime(GameObject target)
    {
        yield return new WaitForSeconds(Duration);
        RemoveEffect(target);
    }
}
