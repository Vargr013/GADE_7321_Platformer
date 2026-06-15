// -----------------------------------------------------------------------------
// Graph.cs
// Tiny undirected Graph ADT. Built once at scene start by WaypointGraph,
// then queried read-only at runtime by BossEnemy.
// It is Generic by design not tied to waypoints 
// -----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    // Internal node table keyed by stable id. Lookup is O(1).
    private readonly Dictionary<string, GraphNode> _nodes = new Dictionary<string, GraphNode>();

    // Number of nodes currently in the graph.
    public int Count => _nodes.Count;

    // Add a node with the given id and payload. Idempotent on duplicates: a repeated id updates the payload rather than creating a second node.
    public void AddNode(string id, Transform waypoint)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("Graph.AddNode called with an empty id. Ignored.");
            return;
        }

        if (_nodes.TryGetValue(id, out GraphNode existing))
        {
            // Update payload in case the same id was added twice with a new transform.
            existing.Waypoint = waypoint;
            return;
        }

        _nodes.Add(id, new GraphNode(id, waypoint));
    }

    // Add an undirected edge between two nodes. If either node is missing, the call is ignored. Idempotent: repeated edges are ignored.
    public void AddEdge(string fromId, string toId)
    {
        if (!_nodes.TryGetValue(fromId, out GraphNode from)) return;
        if (!_nodes.TryGetValue(toId, out GraphNode to)) return;

        // Both directions for an undirected graph.
        from.AddNeighbour(to);
        to.AddNeighbour(from);
    }

    // Look up a node by id. Returns null if not present.
    public GraphNode GetNode(string id)
    {
        _nodes.TryGetValue(id, out GraphNode node);
        return node;
    }

    // Try-get variant: cleaner for callers that want to branch on presence.
    public bool TryGetNode(string id, out GraphNode node)
    {
        return _nodes.TryGetValue(id, out node);
    }

    // True if a node with this id exists in the graph.
    public bool Contains(string id)
    {
        return _nodes.ContainsKey(id);
    }

    // Read-only view of every node in the graph. Useful for picking a start node.
    public IReadOnlyCollection<GraphNode> GetAllNodes()
    {
        return _nodes.Values;
    }
}
