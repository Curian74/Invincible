using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] public float coolDown = 0.5f;
    [SerializeField] private int bulletPoolSize = 45;
    private readonly List<GameObject> bulletPool = new List<GameObject>();
    private Animator animator;
    private float coolDownTimer = 0;
    private int currentBulletIndex = 0;
    public bool MultiShot = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }
    }

    void Update()
    {
        coolDownTimer += Time.deltaTime;
        if (Input.GetMouseButton(0) && coolDownTimer >= coolDown)
        {
            Shoot();
            coolDownTimer = 0;
        }
    }

    public void Shoot()
    {
        SoundManager.Instance.PlaySFX(0);
        animator.SetTrigger("shoot");
        GameObject bullet = GetBullet();
        if (bullet == null) return;
        Vector2 shootDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoint.position).normalized;

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.SetActive(true);

        // Set bullet movement direction
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(shootDirection);
        }

        if (MultiShot)
        {
            GameObject bullet2 = GetBullet();
            GameObject bullet3 = GetBullet();
            GameObject bullet4 = GetBullet();
            GameObject bullet5 = GetBullet();

            if (bullet2 != null)
            {
                float angle = 10f * Mathf.Deg2Rad;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                Bullet script = bullet2.GetComponent<Bullet>();
                bullet2.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, angle));
                bullet2.SetActive(true);
                Vector2 rotatedDirection = new Vector2(
    shootDirection.x * cos - shootDirection.y * sin,
    shootDirection.x * sin + shootDirection.y * cos
);
                script.SetDirection(rotatedDirection);
            }
            if (bullet3 != null)
            {
                float angle = 5f * Mathf.Deg2Rad;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                Bullet script = bullet3.GetComponent<Bullet>();
                bullet3.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, angle));
                bullet3.SetActive(true);
                Vector2 rotatedDirection = new Vector2(
    shootDirection.x * cos - shootDirection.y * sin,
    shootDirection.x * sin + shootDirection.y * cos
);
                script.SetDirection(rotatedDirection);
            }
            if (bullet4 != null)
            {
                float angle = -5f * Mathf.Deg2Rad;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                Bullet script = bullet4.GetComponent<Bullet>();
                bullet4.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, angle));
                bullet4.SetActive(true);
                Vector2 rotatedDirection = new Vector2(
    shootDirection.x * cos - shootDirection.y * sin,
    shootDirection.x * sin + shootDirection.y * cos
);
                script.SetDirection(rotatedDirection);
            }
            if (bullet5 != null)
            {
                float angle = -10f * Mathf.Deg2Rad;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                Bullet script = bullet5.GetComponent<Bullet>();
                bullet5.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, angle));
                bullet5.SetActive(true);
                Vector2 rotatedDirection = new Vector2(
    shootDirection.x * cos - shootDirection.y * sin,
    shootDirection.x * sin + shootDirection.y * cos
);
                script.SetDirection(rotatedDirection);
            }
        }
    }


    private GameObject GetBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            int index = (currentBulletIndex + i) % bulletPoolSize;
            if (!bulletPool[index].activeInHierarchy)
            {
                currentBulletIndex = (index + 1) % bulletPoolSize;
                return bulletPool[index];
            }
        }
        return null;
    }
}
