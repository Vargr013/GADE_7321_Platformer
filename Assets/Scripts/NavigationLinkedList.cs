using UnityEngine;

public class NavigationLinkedList : MonoBehaviour
{
    public WaypointNode head;

    public Transform[] waypoints;

    void Awake()
    {
        foreach (Transform wp in waypoints)
        {
            AddWaypoint(wp);
        }
    }

    public void AddWaypoint(Transform wp)
    {
        WaypointNode newNode = new WaypointNode(wp);

        // This will allow the LinkedList to be circular, meaning the last node will point back to the head
        if (head == null)
        {
            head = newNode;
            head.next = head; 
            return;
        }

        WaypointNode temp = head;

        while (temp.next != head)
        {
            temp = temp.next;
        }

        temp.next = newNode;

        // This will make the new node point back to the head, maintaining the circular structure
        newNode.next = head; 
    }

    public WaypointNode GetHead()
    {
        return head;
    }
}
