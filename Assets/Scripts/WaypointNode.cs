using UnityEngine;

public class WaypointNode 
{
    public Transform waypoint;
    public WaypointNode next;

    public WaypointNode(Transform pointer)
    {
        waypoint = pointer;
        next = null;
    }
}
