using UnityEngine;

public class WaypointNode 
{
    // The transform representing the waypoint's position in the scene
    public Transform waypoint;
    // The next waypoint in the path
    public WaypointNode next;

    // Constructor to initialize the waypoint node with a given transform
    public WaypointNode(Transform pointer)
    {
        waypoint = pointer;
        next = null;
    }
}
