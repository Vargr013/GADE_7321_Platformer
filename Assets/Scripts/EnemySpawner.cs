using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private const int PatrolSpawnIndex = 0;
    private const int ProjectileSpawnIndex = 1;
    private const int FirstChargerSpawnIndex = 2;

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
        if (PatrolEnemyPrefab == null || ProjectileEnemyPrefab == null || ChargerEnemyPrefab == null)
        {
            Debug.LogError("EnemySpawner needs patrol, projectile, and charger prefabs assigned.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length < 3)
        {
            Debug.LogError("EnemySpawner needs at least three spawn points: patrol, projectile, and charger.");
            return;
        }

        if (spawnPoints[PatrolSpawnIndex] != null) factory.CreatePatrolEnemy(spawnPoints[PatrolSpawnIndex].position, 3f, 1f);
        else Debug.LogError("Patrol enemy spawn point is missing.");

        if (spawnPoints[ProjectileSpawnIndex] != null) factory.CreateProjectileEnemy(spawnPoints[ProjectileSpawnIndex].position, 1f);
        else Debug.LogError("Projectile enemy spawn point is missing.");

        for (int i = FirstChargerSpawnIndex; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] != null) factory.CreateChargerEnemy(spawnPoints[i].position, 4f, 1.2f);
            else Debug.LogError($"Charger enemy spawn point {i} is missing.");
        }
    }
}
