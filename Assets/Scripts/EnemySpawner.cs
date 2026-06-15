using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private const int PatrolSpawnIndex = 0;
    private const int ProjectileSpawnIndex = 1;
    private const int FirstChargerSpawnIndex = 2;
    // The boss lives one slot after the required trio. Levels that don't have
    // a boss (or haven't wired a boss prefab yet) just leave this slot empty.
    private const int BossSpawnIndex = 3;

    public GameObject PatrolEnemyPrefab;
    public GameObject ProjectileEnemyPrefab;
    public GameObject ChargerEnemyPrefab;
    // Optional. If null, the boss section is silently skipped - keeps the
    // spawner backward compatible with older levels that don't have a boss.
    public GameObject BossEnemyPrefab;

    [Header("Boss Waves")]
    [Tooltip("How many boss enemies to spawn across the level. 1 = single boss, 4 = four bosses in a wave.")]
    public int bossWaveCount = 1;
    [Tooltip("Seconds between successive boss spawns. Each wave starts after the previous, regardless of whether the prior boss is still alive.")]
    public float bossWaveInterval = 15f;

    private AIEnemyFactory factory;
    public Transform[] spawnPoints;

    void Start()
    {
        // Initialize the factory with the enemy prefabs. The boss prefab is
        // optional - the factory accepts null and CreateBossEnemy checks for it.
        factory = new AIEnemyFactory(PatrolEnemyPrefab, ProjectileEnemyPrefab, ChargerEnemyPrefab, BossEnemyPrefab);

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

        // Boss: opt-in. Only spawns when both the prefab and a boss spawn point
        // are wired. Skipped silently otherwise so older levels keep working.
        if (BossEnemyPrefab != null && spawnPoints.Length > BossSpawnIndex && spawnPoints[BossSpawnIndex] != null)
        {
            // Drive the wave logic from a coroutine. bossWaveCount = 1 keeps the
            // old behaviour (single boss at t=0).
            StartCoroutine(SpawnBossWaves());
        }

        // Chargers fill every remaining slot EXCEPT the boss slot - otherwise
        // a charger would spawn on top of the boss.
        for (int i = FirstChargerSpawnIndex; i < spawnPoints.Length; i++)
        {
            if (i == BossSpawnIndex) continue;
            if (spawnPoints[i] != null) factory.CreateChargerEnemy(spawnPoints[i].position, 4f, 1.2f);
            else Debug.LogError($"Charger enemy spawn point {i} is missing.");
        }
    }

    // Coroutine that spawns bossWaveCount bosses, bossWaveInterval seconds apart.
    // The first boss spawns immediately (wave index 0 has no wait). Subsequent
    // waves wait the interval before spawning, so the player has time to engage
    // each one before the next arrives.
    private IEnumerator SpawnBossWaves()
    {
        Transform spawnPoint = spawnPoints[BossSpawnIndex];
        int waves = Mathf.Max(1, bossWaveCount);

        for (int wave = 0; wave < waves; wave++)
        {
            if (wave > 0) yield return new WaitForSeconds(bossWaveInterval);
            factory.CreateBossEnemy(spawnPoint.position, 8f, 1.2f);
        }
    }
}
