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

    private void AssignScenePlayer(AIEnemyBase enemy)
    {
        GameObject scenePlayer = GameObject.FindGameObjectWithTag("Player");
        if (scenePlayer != null) enemy.player = scenePlayer.transform;
    }
}
