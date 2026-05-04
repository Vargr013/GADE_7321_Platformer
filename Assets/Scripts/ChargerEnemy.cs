using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ChargerEnemy : AIEnemyBase
{
    // Serialized fields for tuning in the Unity Inspector
    [SerializeField] private float navMeshSnapDistance = 3f;
    [SerializeField] private float targetSnapDistance = 6f;

    private NavMeshAgent agent;
    private bool hasHitPlayer;
    private bool warnedMissingNavMesh;
    private NavMeshPath pathToPlayer;

    // Initialization: get the NavMeshAgent component, set speed, prepare pathfinding, and ensure the enemy is on the NavMesh
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        pathToPlayer = new NavMeshPath();
        TryPlaceOnNavMesh();
        BindScenePlayer();
    }

    private void Update()
    {
        Act();
    }

    // Core behavior: if the player is reachable on the NavMesh, move towards them; otherwise, stop moving
    public override void Act()
    {
        if (!HasScenePlayer()) BindScenePlayer();
        if (agent == null || player == null) return;
        if (!EnsureAgentOnNavMesh()) return;

        if (CanReachPlayerOnPlatform(out Vector3 destination))
        {
            agent.isStopped = false;
            agent.SetDestination(destination);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    // Helper methods for player binding, NavMesh validation, pathfinding, and collision handling
    private void BindScenePlayer()
    {
        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null) player = foundPlayer.transform;
    }

    private bool HasScenePlayer()
    {
        return player != null && player.gameObject.scene.IsValid() && player.gameObject.scene.isLoaded;
    }

    private bool EnsureAgentOnNavMesh()
    {
        if (!agent.enabled) return false;
        if (agent.isOnNavMesh) return true;

        if (TryPlaceOnNavMesh()) return true;

        if (!warnedMissingNavMesh)
        {
            Debug.LogWarning($"{name} is not on a NavMesh. Move it onto baked navigation or rebake the level NavMesh.", this);
            warnedMissingNavMesh = true;
        }

        return false;
    }

    private bool TryPlaceOnNavMesh()
    {
        if (agent == null || agent.isOnNavMesh) return agent != null && agent.isOnNavMesh;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, navMeshSnapDistance, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            warnedMissingNavMesh = false;
            return true;
        }

        return false;
    }

    // Determines if the player is within detection range and reachable on the NavMesh, returning the valid destination if so
    private bool CanReachPlayerOnPlatform(out Vector3 destination)
    {
        destination = player.position;

        if (Vector3.Distance(transform.position, player.position) > detectionRange)
        {
            return false;
        }

        if (!NavMesh.SamplePosition(player.position, out NavMeshHit hit, targetSnapDistance, NavMesh.AllAreas))
        {
            return false;
        }

        destination = hit.position;

        return agent.CalculatePath(destination, pathToPlayer) && pathToPlayer.status == NavMeshPathStatus.PathComplete;
    }

    // Robust implementation of collision handling to ensure the player is only hit once, regardless of trigger type
    private void OnCollisionEnter(Collision collision)
    {
        TryHitPlayer(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHitPlayer(other);
    }

    // Helper method to handle player collision logic, ensuring only one hit is registered within a short time frame
    private void TryHitPlayer(Collider other)
    {
        if (hasHitPlayer || !other.CompareTag("Player")) return;

        PlayerController playerController = other.GetComponentInParent<PlayerController>();
        if (playerController == null) return;

        hasHitPlayer = true;
        playerController.playerHit();
        Invoke(nameof(ResetHit), 0.5f);
    }

    // Resets the hit state after a short delay to allow for subsequent hits if the player remains in contact
    private void ResetHit()
    {
        hasHitPlayer = false;
    }
}
