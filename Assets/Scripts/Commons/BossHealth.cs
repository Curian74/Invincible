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
		protected override void Die()
		{
			Debug.Log("Overriden");
			_animator.SetTrigger("Death");
			StartCoroutine(DieAfterDelay());
		}

		private IEnumerator DieAfterDelay()
		{
			yield return new WaitForSeconds(1f);
			base.Die();
		}
	}
}
