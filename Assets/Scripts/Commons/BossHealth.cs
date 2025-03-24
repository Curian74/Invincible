using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Commons
{
	public class BossHealth : Health
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
}
