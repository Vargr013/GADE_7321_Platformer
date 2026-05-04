using UnityEngine;

public class ProjectileEnemy : AIEnemyBase
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public float rotationSpeed = 5f;

    private float timer = 0f;

    void Update()
    {
        if (player == null) return;

        bool canSeePlayer = CanSeePlayer();

        // Shoot only if the player is visible and in range and toward the player
        if (canSeePlayer)
        {
            RotateTowardsPlayer();
            HandleShooting();
        }
    }

    // Detection with line of sight
    bool CanSeePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
            return false;

        RaycastHit hit;

        // Start the raycast from the enemy's head (or a point above its position) to the player's head
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 target = player.position + Vector3.up * 1f;
        Vector3 direction = (target - origin).normalized;

        Debug.DrawRay(origin, direction * detectionRange, Color.red);

        if (Physics.Raycast(origin, direction, out hit, detectionRange))
        {
            if (hit.transform.root == player)
                return true;
        }

        return false;
    }

    // Smooth rotation toward player
    void RotateTowardsPlayer()
    {
        // Calculate direction to player and ignore vertical difference
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; 

        if (direction == Vector3.zero) return;

        // Calculate target rotation and smoothly rotate towards it
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Shooting logic with cooldown
    void HandleShooting()
    {
        timer += Time.deltaTime;

        if (timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    // Method to instantiate the projectile
    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // Calculate direction to player and set it for the projectile
        Vector3 direction = (player.position - firePoint.position).normalized;

        projectile.GetComponent<Projectile>().SetDirection(direction);
    }

    public override void Act()
    {
        // Shooting handled in Update
    }
}
