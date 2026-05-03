using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject PatrolEnemyPrefab;
    public GameObject ProjectileEnemyPrefab;

    private AIEnemyFactory factory;

    void Start()
    {
        // Initialize the factory with the enemy prefabs
        factory = new AIEnemyFactory(PatrolEnemyPrefab, ProjectileEnemyPrefab);

        // Spawn patrol enemy
        factory.CreatePatrolEnemy(new Vector3(0, 0, 0), 3f, 1f);

        // Spawn projectile enemy
        factory.CreateProjectileEnemy(new Vector3(5, 0, 5), 1.5f);
    }
}
