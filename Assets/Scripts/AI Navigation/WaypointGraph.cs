using System.Collections.Generic;
using UnityEngine;

public class WaypointGraph : MonoBehaviour
{
    [Header("Node Setup")]
    [Tooltip("Scene transforms that act as waypoint positions. Order must match nodeIds.")]
    [SerializeField] private Transform[] _waypointTransforms;

    [Tooltip("Stable string id for each waypoint")]
    [SerializeField] private string[] _nodeIds;

    [Header("Edges")]
    [Tooltip("Undirected edges between nodes. Format: \"A-B\". ")]
    [SerializeField] private string[] _edges = new string[0];

    // The fully built graph. Read-only from outside.
    public Graph Graph { get; private set; }

    // Build the graph once when the scene loads.
    private void Awake()
    {
        BuildGraph();
    }

    // Construct the Graph from Inspector data. Safe to call multiple times - the underlying containers are recreated each time.
    private void BuildGraph()
    {
        Graph = new Graph();

        // Validation: if the arrays are null or mismatched, log a warning and leave the graph empty.
        if (_waypointTransforms == null || _nodeIds == null)
        {
            Debug.LogWarning($"[{nameof(WaypointGraph)}] Waypoint arrays are null. Graph will be empty.", this);
            return;
        }

        if (_waypointTransforms.Length != _nodeIds.Length)
        {
            Debug.LogWarning(
                $"[{nameof(WaypointGraph)}] waypointTransforms.Length ({_waypointTransforms.Length}) " +
                $"!= nodeIds.Length ({_nodeIds.Length}). Graph will be empty.", this);
            return;
        }

        // Add every node first so edges can resolve their endpoints.
        for (int i = 0; i < _waypointTransforms.Length; i++)
        {
            Graph.AddNode(_nodeIds[i], _waypointTransforms[i]);
        }

        // parse and add each edge. Format is "from-to" using '-' as a separator. Whitespace is trimmed so "A - B" works too.
        if (_edges != null)
        {
            foreach (string raw in _edges)
            {
                if (string.IsNullOrWhiteSpace(raw)) continue;

                string[] parts = raw.Split('-');
                if (parts.Length != 2)
                {
                    Debug.LogWarning($"[{nameof(WaypointGraph)}] Edge '{raw}' is not in 'A-B' format. Skipped.", this);
                    continue;
                }

                string from = parts[0].Trim();
                string to = parts[1].Trim();

                if (!Graph.Contains(from) || !Graph.Contains(to))
                {
                    Debug.LogWarning(
                        $"[{nameof(WaypointGraph)}] Edge '{raw}' references unknown node id. Skipped.", this);
                    continue;
                }

                Graph.AddEdge(from, to);
            }
        }
    }

    // Pick a uniformly random neighbour of the given node id.
    // Returns the neighbour's id, or null if the node is unknown / has no neighbours.
    public string GetRandomNeighbour(string id)
    {
        if (Graph == null) return null;
        if (!Graph.TryGetNode(id, out GraphNode node)) return null;
        if (node.Neighbours == null || node.Neighbours.Count == 0) return null;

        int index = Random.Range(0, node.Neighbours.Count);
        return node.Neighbours[index].Id;
    }
}
