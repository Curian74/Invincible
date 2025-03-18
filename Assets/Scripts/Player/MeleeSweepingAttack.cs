using UnityEngine;

public class MeleeSweepingAttack : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackAngle = 90f;
    public int rayCount = 8;
    public LayerMask enemyLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    void Attack()
    {
        float startAngle = -attackAngle / 2;
        float angleStep = attackAngle / (rayCount - 1);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Quaternion attackDirection = gameObject.transform.rotation.normalized;

        float baseAngle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = baseAngle + startAngle + (angleStep * i);
            Vector3 rayDirection = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, attackRange, enemyLayer);
            Debug.DrawRay(transform.position, rayDirection * attackRange, Color.red, 0.1f);

            if (hit.collider != null)
            {
                Debug.Log("Hit: " + hit.collider.name);
            }
        }
    }
}
