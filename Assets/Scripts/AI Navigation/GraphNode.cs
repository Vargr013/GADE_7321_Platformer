// -----------------------------------------------------------------------------
// GraphNode.cs
// Lightweight data node for the boss waypoint Graph ADT.
// Used by WaypointGraph  and BossEnemy.
// -----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

public class GraphNode
{
    // Stable identifier (e.g. "A", "B"...). Lookups in the Graph go through this.
    public string Id;

    // Scene transform the boss can navigate to.
    public Transform Waypoint;

    // Adjacency list - all nodes connected to this one by a single edge.
    // Undirected, so if A neighbours B, then B neighbours A.
    public List<GraphNode> Neighbours;

    public GraphNode(string id, Transform waypoint)
    {
        Id = id;
        Waypoint = waypoint;
        Neighbours = new List<GraphNode>();
    }

    // Add a neighbour if it isn't already linked. Returns true if the link was new.
    public bool AddNeighbour(GraphNode n)
    {
        if (n == null) return false;
        if (Neighbours.Contains(n)) return false;
        Neighbours.Add(n);
        return true;
    }
}
