// BossEnemy.cs - Boss enemy that patrols the WaypointGraph and randomly branches at every node.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossEnemy : AIEnemyBase
{
    [Tooltip("Scene WaypointGraph the boss will patrol. Assigned by AIEnemyFactory at spawn.")]
    [SerializeField] private WaypointGraph _waypointGraph;
    [Tooltip("Seconds the boss may spend on a single destination without making progress before it picks a different neighbour.")]
    [SerializeField] private float _stuckTimeout = 5f;
    [Tooltip("Distance the boss must travel during the stuck window to count as making progress.")]
    [SerializeField] private float _stuckDistanceThreshold = 0.5f;
    [Tooltip("Minimum delay between successive player hits, to prevent per-frame damage.")]
    [SerializeField] private float hitCooldown = 0.5f;

    private NavMeshAgent _agent;
    private NavMeshPath _pathCheckBuffer;
    private float _stuckCheckStartTime;
    private Vector3 _stuckCheckStartPos;
    private string _currentNodeId;
    private bool _hasHitPlayer;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = speed;
        _pathCheckBuffer = new NavMeshPath();
        TryStartPatrol();
    }

    // Public seam used by AIEnemyFactory to inject the scene's WaypointGraph.
    public void AssignWaypointGraph(WaypointGraph graph)
    {
        _waypointGraph = graph;
        if (_agent != null && string.IsNullOrEmpty(_currentNodeId)) TryStartPatrol();
    }

    private void Update()
    {
        if (_waypointGraph == null || _waypointGraph.Graph == null) return;
        if (string.IsNullOrEmpty(_currentNodeId)) { TryStartPatrol(); return; }

        // Stuck retry: if we haven't moved in a while, try a different neighbour.
        if (Time.time - _stuckCheckStartTime > _stuckTimeout
            && Vector3.Distance(transform.position, _stuckCheckStartPos) < _stuckDistanceThreshold)
        {
            GoToRandomNeighbour();
            return;
        }

        if (_agent.pathPending) return;
        if (_agent.remainingDistance > _agent.stoppingDistance) return;

        GoToRandomNeighbour();
    }

    private void TryStartPatrol()
    {
        if (_waypointGraph == null || _waypointGraph.Graph == null) return;
        IReadOnlyCollection<GraphNode> all = _waypointGraph.Graph.GetAllNodes();
        if (all == null || all.Count == 0)
        {
            Debug.LogWarning($"[{nameof(BossEnemy)}] WaypointGraph is empty - no nodes to patrol.", this);
            return;
        }
        foreach (GraphNode node in all)
        {
            _currentNodeId = node.Id;
            if (node.Waypoint != null) _agent.SetDestination(node.Waypoint.position);
            _stuckCheckStartTime = Time.time;
            _stuckCheckStartPos = transform.position;
            break;
        }
    }

    // Try neighbours in random order, skipping any without a complete NavMesh path.
    public void GoToRandomNeighbour()
    {
        if (_waypointGraph == null || _waypointGraph.Graph == null) return;
        if (string.IsNullOrEmpty(_currentNodeId)) return;
        if (!_waypointGraph.Graph.TryGetNode(_currentNodeId, out GraphNode currentNode)) return;
        if (currentNode.Neighbours == null || currentNode.Neighbours.Count == 0) return;

        List<GraphNode> candidates = new List<GraphNode>(currentNode.Neighbours);
        Shuffle(candidates);
        foreach (GraphNode candidate in candidates)
        {
            if (candidate == null || candidate.Waypoint == null) continue;
            if (!HasReachablePath(candidate.Waypoint.position)) continue;
            _currentNodeId = candidate.Id;
            _agent.SetDestination(candidate.Waypoint.position);
            _stuckCheckStartTime = Time.time;
            _stuckCheckStartPos = transform.position;
            return;
        }
        Debug.LogWarning($"[{nameof(BossEnemy)}] No reachable neighbour from {_currentNodeId} - staying put.", this);
    }

    // Checks if the NavMeshAgent can reach the target position with a complete path.
    private bool HasReachablePath(Vector3 target)
    {
        if (_pathCheckBuffer == null) _pathCheckBuffer = new NavMeshPath();
        return _agent.CalculatePath(target, _pathCheckBuffer)
            && _pathCheckBuffer.status == NavMeshPathStatus.PathComplete;
    }
    // Fisher-Yates shuffle to randomize neighbour order.
    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    // Same as GoToRandomNeighbour; kept for WaypointTrigger.SendMessage compat.
    public void MoveToNextWaypoint() => GoToRandomNeighbour();

    private void OnCollisionEnter(Collision collision) => TryHitPlayer(collision.collider);
    private void OnTriggerEnter(Collider other) => TryHitPlayer(other);

    // Attempts to hit the player if we collide with them, respecting the hit cooldown.
    private void TryHitPlayer(Collider other)
    {
        if (_hasHitPlayer || other == null || !other.CompareTag("Player")) return;
        PlayerController playerController = other.GetComponentInParent<PlayerController>();
        if (playerController == null) return;
        _hasHitPlayer = true;
        playerController.playerHit();
        Invoke(nameof(ResetHit), hitCooldown);
    }
    // Resets the hit flag after the cooldown, allowing the boss to damage the player again.
    private void ResetHit() => _hasHitPlayer = false;

    // Required by AIEnemyBase; behaviour is driven from Update.
    public override void Act() { }
}
