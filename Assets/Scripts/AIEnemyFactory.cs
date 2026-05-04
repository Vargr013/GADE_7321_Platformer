using UnityEngine;

public class AIEnemyFactory : EnemyFactory
{
    private GameObject PatrolEnemyPrefab;
    private GameObject ProjectileEnemyPrefab;
    private GameObject ChargerEnemyPrefab;

    public AIEnemyFactory(GameObject patrol, GameObject projectile, GameObject charger)
    {
        PatrolEnemyPrefab = patrol;
        ProjectileEnemyPrefab = projectile;
        ChargerEnemyPrefab = charger;
    }

    public override AIEnemyBase CreateEnemy(Vector3 position)
    {
        GameObject obj = GameObject.Instantiate(PatrolEnemyPrefab, position, Quaternion.identity);
        return obj.GetComponent<AIEnemyBase>();
    }

    // Will create a patrol enemy with the specified speed and size.
    public AIEnemyBase CreatePatrolEnemy(Vector3 position, float speed, float size)
    {
        GameObject obj = GameObject.Instantiate(PatrolEnemyPrefab, position, Quaternion.identity);
        AIEnemyBase enemy = obj.GetComponent<AIEnemyBase>();
        enemy.Initialize(speed, size);
        return enemy;
    }

    // Will create a charger enemy with the specified speed and size.
    public AIEnemyBase CreateChargerEnemy(Vector3 position, float speed, float size)
    {
        GameObject obj = GameObject.Instantiate(ChargerEnemyPrefab, position, Quaternion.identity);
        AIEnemyBase enemy = obj.GetComponent<AIEnemyBase>();
        enemy.Initialize(speed, size);
        return enemy;
    }

    // Will create a projectile enemy with a default speed of 0, as it may not move but rather shoot projectiles.
    public AIEnemyBase CreateProjectileEnemy(Vector3 position, float size)
    {
        GameObject obj = GameObject.Instantiate(ProjectileEnemyPrefab, position, Quaternion.identity);
        AIEnemyBase enemy = obj.GetComponent<AIEnemyBase>();
        enemy.Initialize(0f, size);
        return enemy;
    }
}
