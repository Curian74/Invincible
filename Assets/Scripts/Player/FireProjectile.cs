using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private Transform firePoint; // Assign a child GameObject positioned at the top of the player

    [SerializeField]
    private float fireRate = 0.5f; // Time in seconds between shots

    [SerializeField]
    private int poolSize = 25;

    private List<GameObject> projectilePool;

    private float nextFireTime = 0f; // Tracks when the player can fire next

    private int currentIndex = 0;

    [SerializeField]
    private Animator animator;

    private void Start()
    {
        projectilePool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            projectile.SetActive(false);
            projectilePool.Add(projectile);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        GameObject projectile = null;
        animator.SetTrigger("Fire");

        for (int i = 0; i < poolSize; i++)
        {
            int index = (currentIndex + i) % poolSize;
            Debug.Log($"{currentIndex}/{index}/{poolSize - 1}");
            if (!projectilePool[index].activeInHierarchy)
            {
                projectile = projectilePool[index];
                projectile.transform.position = firePoint.position;
                projectile.SetActive(true);
                currentIndex = index;
                break;
            }
        }

        if (projectile != null)
        {
            Bullet projScript = projectile.GetComponent<Bullet>();

            if (projScript != null)
            {
                // Make the projectile fly in the direction the player is facing
                Vector2 shootDirection = firePoint.up;
                projScript.SetDirection(shootDirection);
            }
        }
    }
}