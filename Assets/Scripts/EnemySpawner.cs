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
        if (spawnPoints == null || spawnPoints.Length < 3)
        {
            Debug.LogError("EnemySpawner needs three spawn points: patrol, charger, and projectile.");
            return;
        }

        // Patrol enemies
        if (spawnPoints[0] != null) factory.CreatePatrolEnemy(spawnPoints[0].position, 3f, 1f);
        else Debug.LogError("Patrol enemy spawn point is missing.");

        // Charger enemy
        if (spawnPoints[1] != null) factory.CreateChargerEnemy(spawnPoints[1].position, 4f, 1.2f);
        else Debug.LogError("Charger enemy spawn point is missing.");

        // Projectile enemy
        if (spawnPoints[2] != null) factory.CreateProjectileEnemy(spawnPoints[2].position, 1f);
        else Debug.LogError("Projectile enemy spawn point is missing.");
    }
}
