using System.Collections;
using UnityEngine;

public class EnemyDestroyController : MonoBehaviour
{
   public void DestroyEnemy(float delay)
{
    StartCoroutine(DeactivateEnemy(delay));
}

private IEnumerator DeactivateEnemy(float delay)
{
    yield return new WaitForSeconds(delay);
    gameObject.SetActive(false); 
}

}