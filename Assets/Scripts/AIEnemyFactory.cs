using UnityEngine;

public class AIEnemyFactory : EnemyFactory
{
    private GameObject PatrolEnemyPrefab;
    private GameObject ProjectileEnemyPrefab;
    private GameObject ChargerEnemyPrefab;
    private GameObject BossEnemyPrefab;

    public AIEnemyFactory(GameObject patrol, GameObject projectile, GameObject charger, GameObject boss = null)
    {
        PatrolEnemyPrefab = patrol;
        ProjectileEnemyPrefab = projectile;
        ChargerEnemyPrefab = charger;
        // Boss is optional for now - the spawner doesn't wire it in yet, but callers that want a boss can pass a prefab and call CreateBossEnemy.
        BossEnemyPrefab = boss;
    }

    public override AIEnemyBase CreateEnemy(Vector3 position)
    {
        if (PatrolEnemyPrefab == null)
        {
            Debug.LogError("Cannot create default enemy because PatrolEnemyPrefab is missing.");
            return null;
        }

        GameObject obj = GameObject.Instantiate(PatrolEnemyPrefab, position, Quaternion.identity);
        AIEnemyBase enemy = obj.GetComponent<AIEnemyBase>();
        if (enemy != null) AssignScenePlayer(enemy);
        return enemy;
    }

    // Will create a patrol enemy with the specified speed and size.
    public AIEnemyBase CreatePatrolEnemy(Vector3 position, float speed, float size)
    {
        if (PatrolEnemyPrefab == null)
        {
            Debug.LogError("Cannot create patrol enemy because PatrolEnemyPrefab is missing.");
            return null;
        }

        GameObject obj = GameObject.Instantiate(PatrolEnemyPrefab, position, Quaternion.identity);
        AIEnemyBase enemy = obj.GetComponent<AIEnemyBase>();
        if (enemy != null)
        {
            enemy.Initialize(speed, size);
            AssignScenePlayer(enemy);

            PatrolEnemy patrol = obj.GetComponent<PatrolEnemy>();
            NavigationLinkedList waypoints = GameObject.FindFirstObjectByType<NavigationLinkedList>();
            if (patrol != null && waypoints != null) patrol.waypointList = waypoints;
        }
        else Debug.LogError("PatrolEnemyPrefab does not have an AIEnemyBase component.");
        return enemy;
    }

    // Will create a charger enemy with the specified speed and size.
    public AIEnemyBase CreateChargerEnemy(Vector3 position, float speed, float size)
    {
        if (ChargerEnemyPrefab == null)
        {
            Debug.LogError("Cannot create charger enemy because ChargerEnemyPrefab is missing.");
            return null;
        }

        GameObject obj = GameObject.Instantiate(ChargerEnemyPrefab, position, Quaternion.identity);
        AIEnemyBase enemy = obj.GetComponent<AIEnemyBase>();
        if (enemy != null)
        {
            enemy.Initialize(speed, size);
            AssignScenePlayer(enemy);
        }
        else Debug.LogError("ChargerEnemyPrefab does not have an AIEnemyBase component.");
        return enemy;
    }

    // Will create a projectile enemy with a default speed of 0, as it may not move but rather shoot projectiles.
    public AIEnemyBase CreateProjectileEnemy(Vector3 position, float size)
    {
        if (ProjectileEnemyPrefab == null)
        {
            Debug.LogError("Cannot create projectile enemy because ProjectileEnemyPrefab is missing.");
            return null;
        }

        GameObject obj = GameObject.Instantiate(ProjectileEnemyPrefab, position, Quaternion.identity);
        AIEnemyBase enemy = obj.GetComponent<AIEnemyBase>();
        if (enemy != null)
        {
            enemy.Initialize(0f, size);
            AssignScenePlayer(enemy);
        }
        else Debug.LogError("ProjectileEnemyPrefab does not have an AIEnemyBase component.");
        return enemy;
    }

    // Will create a boss enemy. The boss walks the WaypointGraph, so the
    // factory resolves the graph at spawn time and hands it to the enemy.
    public AIEnemyBase CreateBossEnemy(Vector3 position, float speed, float size)
    {
        if (BossEnemyPrefab == null)
        {
            Debug.LogError("Cannot create boss enemy because BossEnemyPrefab is missing.");
            return null;
        }

        // Instantiate the prefab and initialise base stats / scale.
        GameObject obj = GameObject.Instantiate(BossEnemyPrefab, position, Quaternion.identity);
        AIEnemyBase enemy = obj.GetComponent<AIEnemyBase>();
        if (enemy == null)
        {
            Debug.LogError("BossEnemyPrefab does not have an AIEnemyBase component.");
            return null;
        }

        enemy.Initialize(speed, size);
        AssignScenePlayer(enemy);

        // Find the scene's WaypointGraph and hand it to the boss so it knows where to patrol
        WaypointGraph graph = Object.FindFirstObjectByType<WaypointGraph>();
        if (graph == null)
        {
            Debug.LogError(
                "No WaypointGraph found in the scene. " +
                "Add a WaypointGraph component to a GameObject and populate its Inspector arrays.", obj);
            return enemy;
        }

        BossEnemy boss = obj.GetComponent<BossEnemy>();
        if (boss != null)
        {
            // Assign the graph to the boss. If Start has already run, this will kick off the patrol immediately.
            boss.AssignWaypointGraph(graph);
        }
        else
        {
            Debug.LogError("BossEnemyPrefab does not have a BossEnemy component.", obj);
        }

        return enemy;
    }

    // Assign the scene's player to the enemy so it can chase / attack. If no player is found, the enemy will idle.
    private void AssignScenePlayer(AIEnemyBase enemy)
    {
        GameObject scenePlayer = GameObject.FindGameObjectWithTag("Player");
        if (scenePlayer != null) enemy.player = scenePlayer.transform;
    }
}
