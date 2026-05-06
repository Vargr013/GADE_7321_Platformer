using UnityEngine;

public class NavigationLinkedList : MonoBehaviour
{
    public WaypointNode head;

    public Transform[] waypoints;

    void Awake()
    {
        // Initialize the linked list with the waypoints provided in the inspector
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

        // Traverse the list to find the last node (the one that points back to the head)
        while (temp.next != head)
        {
            temp = temp.next;
        }

        temp.next = newNode;

        // This will make the new node point back to the head, maintaining the circular structure
        newNode.next = head; 
    }

    // This method can be used to retrieve the head of the linked list, which is the starting point for navigation
    public WaypointNode GetHead()
    {
        return head;
    }
}
