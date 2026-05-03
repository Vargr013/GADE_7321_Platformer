using UnityEngine;

public class ProjectileEnemy : AIEnemyBase
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        // Check if it's time to shoot
        if (timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    // Method to instantiate the projectile
    void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }

    public override void Act()
    {
        // Shooting handled in Update
    }
}
