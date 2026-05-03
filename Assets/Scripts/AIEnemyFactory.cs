using UnityEngine;

public class AIEnemyFactory : EnemyFactory
{
    private GameObject PatrolEnemyPrefab;
    private GameObject ProjectileEnemyPrefab;

    public AIEnemyFactory(GameObject patrol, GameObject projectile)
    {
        PatrolEnemyPrefab = patrol;
        ProjectileEnemyPrefab = projectile;
    }

    public override AIEnemyBase CreateEnemy(Vector3 position)
    {
        GameObject obj = GameObject.Instantiate(PatrolEnemyPrefab, position, Quaternion.identity);
        return obj.GetComponent<AIEnemyBase>();
    }

    public AIEnemyBase CreatePatrolEnemy(Vector3 position, float speed, float size)
    {
        GameObject obj = GameObject.Instantiate(PatrolEnemyPrefab, position, Quaternion.identity);
        AIEnemyBase enemy = obj.GetComponent<AIEnemyBase>();
        enemy.Initialize(speed, size);
        return enemy;
    }

    public AIEnemyBase CreateProjectileEnemy(Vector3 position, float size)
    {
        GameObject obj = GameObject.Instantiate(ProjectileEnemyPrefab, position, Quaternion.identity);
        AIEnemyBase enemy = obj.GetComponent<AIEnemyBase>();
        enemy.Initialize(0f, size);
        return enemy;
    }
}
