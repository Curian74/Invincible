using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float coolDown = 0.5f;
    [SerializeField] private int bulletPoolSize = 30;
    private readonly List<GameObject> bulletPool = new List<GameObject>();
    private Animator animator;
    private float coolDownTimer = 0;
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
        animator.SetTrigger("shoot");
        GameObject bullet = GetBullet();
        if (bullet == null) return;

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.SetActive(true);

        // Set bullet movement direction
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            Vector2 shootDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoint.position).normalized;
            bulletScript.SetDirection(shootDirection);
        }
    }


    private GameObject GetBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (!bulletPool[i].activeInHierarchy)
            {
                return bulletPool[i];
            }
        }
        return null;
    }
}
