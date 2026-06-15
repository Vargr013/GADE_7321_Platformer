using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossEnemy : AIEnemyBase
{
    [Header("Path")]
    [Tooltip("Scene WaypointGraph the boss will patrol. Assigned by AIEnemyFactory at spawn.")]
    [SerializeField] private WaypointGraph _waypointGraph;

    [Header("Combat")]
    [Tooltip("Minimum delay between successive player hits, to prevent per-frame damage.")]
    [SerializeField] private float hitCooldown = 0.5f;

    // Cached NavMeshAgent.
    private NavMeshAgent _agent;

    // Id of the node the boss is currently heading to / standing on.
    private string _currentNodeId;

    // True after the boss has hit the player, until the cooldown elapses.
    private bool _hasHitPlayer;

    // finds a starting node, and issues the first SetDestination. Safe to run when the graph is empty -in that case we log once and idle, then keep re-checking from Update.
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = speed;

        TryStartPatrol();
    }

    // Called by AIEnemyFactory after the boss is spawned, to wire up the WaypointGraph. If Start has already run, this will kick off the patrol immediately.
    public void AssignWaypointGraph(WaypointGraph graph)
    {
        _waypointGraph = graph;

        // If Start has already run with no graph, kick off the patrol now.
        if (_agent != null && string.IsNullOrEmpty(_currentNodeId))
        {
            TryStartPatrol();
        }
    }

    // If the boss has reached its current destination, pick a random neighbour and head there.
    private void Update()
    {
        // No graph wired? Nothing to do.
        if (_waypointGraph == null || _waypointGraph.Graph == null) return;

        // Graph became empty at the time of Start (developer hasn't filled
        // the Inspector yet). Re-try every frame; TryStartPatrol is cheap.
        if (string.IsNullOrEmpty(_currentNodeId))
        {
            TryStartPatrol();
            return;
        }

        if (_agent.pathPending) return;
        if (_agent.remainingDistance > _agent.stoppingDistance) return;

        GoToRandomNeighbour();
    }

    // Pick the first node in the graph as the starting point. If the graph is empty, it will log a warning and idle 
    private void TryStartPatrol()
    {
        if (_waypointGraph == null || _waypointGraph.Graph == null) return;

        IReadOnlyCollection<GraphNode> all = _waypointGraph.Graph.GetAllNodes();
        if (all == null || all.Count == 0)
        {
            Debug.LogWarning(
                $"[{nameof(BossEnemy)}] WaypointGraph is empty - no nodes to patrol. " +
                "Fill the Inspector arrays on the WaypointGraph component.", this);
            return;
        }

        // Pick the first node in the graph and head there. The graph is unordered, so this is effectively a random starting point.
        foreach (GraphNode node in all)
        {
            _currentNodeId = node.Id;
            if (node.Waypoint != null) _agent.SetDestination(node.Waypoint.position);
            break;
        }
    }

    // Pick a random neighbour of the current node and head there. If the current node has no neighbours, the boss will idle until Update re-checks.
    public void GoToRandomNeighbour()
    {
        if (_waypointGraph == null || _waypointGraph.Graph == null) return;
        if (string.IsNullOrEmpty(_currentNodeId)) return;

        string next = _waypointGraph.GetRandomNeighbour(_currentNodeId);
        if (string.IsNullOrEmpty(next)) return;   // dead-end or unknown id

        GraphNode nextNode = _waypointGraph.Graph.GetNode(next);
        if (nextNode == null || nextNode.Waypoint == null) return;

        _currentNodeId = next;
        _agent.SetDestination(nextNode.Waypoint.position);
    }

    // Move to the next waypoint in the patrol. This is called by the patrol script family, which is why it exists even though the boss's behaviour is driven from Update.
    public void MoveToNextWaypoint()
    {
        GoToRandomNeighbour();
    }

    // ----- Combat ----------------------------------------------------------
    // Local copy of ChargerEnemy's hit logic. Kept here (not inherited) so
    // the two enemy types remain independent. Same flags, same cooldown.
    // ------------------------------------------------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        TryHitPlayer(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHitPlayer(other);
    }

    // Attempt to damage the player. Respects the hit cooldown so the player isn't damaged every frame while touching the boss.
    private void TryHitPlayer(Collider other)
    {
        if (_hasHitPlayer || other == null || !other.CompareTag("Player")) return;

        PlayerController playerController = other.GetComponentInParent<PlayerController>();
        if (playerController == null) return;

        _hasHitPlayer = true;
        playerController.playerHit();
        Invoke(nameof(ResetHit), hitCooldown);
    }

    // Clear the hit flag after the cooldown so subsequent contacts can damage again.
    private void ResetHit()
    {
        _hasHitPlayer = false;
    }

    // Required by AIEnemyBase. The boss's behaviour is driven from Update (matching the rest of the patrol script family), so this is intentionally empty.
    public override void Act()
    {
    }
}
