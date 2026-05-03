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

        if (timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }

    public override void Act()
    {
        // Shooting handled in Update
    }
}
