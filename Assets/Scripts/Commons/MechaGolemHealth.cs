using System.Collections;
using UnityEngine;

public class MechaGolemHealth : Health
{
    [SerializeField] private AudioClip _deathSound;
    protected override void Die()
    {
        Debug.Log("Overriden");
        _animator.SetTrigger("Death");
        SoundManager.Instance.PlaySFX(_deathSound);
        _rb.simulated = false;
        StartCoroutine(DieAfterDelay());
    }

    private IEnumerator DieAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        base.Die();
    }
}
