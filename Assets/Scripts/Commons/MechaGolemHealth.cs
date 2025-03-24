using System.Collections;
using UnityEngine;

public class MechaGolemHealth : Health
{
    [SerializeField] private AudioClip _deathSound;
    protected override void Die()
    {
        _rb.simulated = false;
        Debug.Log("Overriden");
        _animator.SetTrigger("Death");
        SoundManager.Instance.PlaySFX(_deathSound);
        
        StartCoroutine(DieAfterDelay());
    }

    private IEnumerator DieAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        base.Die();
    }

    private void OnEnable()
    {
        maxHealth += 20f;
        Heal(maxHealth);
        Debug.Log(maxHealth);
    }
}
