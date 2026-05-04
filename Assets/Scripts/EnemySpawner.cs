using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject PatrolEnemyPrefab;
    public GameObject ProjectileEnemyPrefab;
    public GameObject ChargerEnemyPrefab;

    private AIEnemyFactory factory;
    public Transform[] spawnPoints;

    void Start()
    {
        // Initialize the factory with the enemy prefabs
        factory = new AIEnemyFactory(PatrolEnemyPrefab, ProjectileEnemyPrefab, ChargerEnemyPrefab);

        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        // Patrol enemies
        factory.CreatePatrolEnemy(spawnPoints[0].position, 3f, 1f);

        // Charger enemy
        factory.CreateChargerEnemy(spawnPoints[1].position, 4f, 1.2f);
        // Projectile enemy
        factory.CreateProjectileEnemy(spawnPoints[2].position, 1f);
    }
}
