using UnityEngine;
using UnityEngine.AI;


public class AIEnemyPatrol : MonoBehaviour
{
    public NavigationLinkedList waypointList;
    private WaypointNode currentNode;

    private NavMeshAgent agent;

    void Start()
    {
        // Initialize the NavMeshAgent and set the first destination
        agent = GetComponent<NavMeshAgent>();
        if (waypointList == null)
        {
            Debug.LogError("WaypointList not assigned!");
            return;
        }

        // Start at the head of the linked list
        currentNode = waypointList.GetHead();
        if (currentNode == null)
        {
            Debug.LogError("LinkedList is empty!");
            return;
        }

        // Set the initial destination to the first waypoint
        if (currentNode != null)
        {
            agent.SetDestination(currentNode.waypoint.position);
        }
    }

    void Update()
    {
        // Check if the agent has reached the current waypoint
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            MoveToNextWaypoint();
        }
    }

    // Move to the next waypoint in the linked list
    void MoveToNextWaypoint()
    {
        if (currentNode == null) return;

        currentNode = currentNode.next;
        agent.SetDestination(currentNode.waypoint.position);
    }
}
