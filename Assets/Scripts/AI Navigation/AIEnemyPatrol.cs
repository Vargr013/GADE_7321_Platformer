using UnityEngine;
using UnityEngine.AI;


public class AIEnemyPatrol : MonoBehaviour
{
    public NavigationLinkedList waypointList;
    private WaypointNode currentNode;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (waypointList == null)
        {
            Debug.LogError("WaypointList not assigned!");
            return;
        }

        currentNode = waypointList.GetHead();
        if (currentNode == null)
        {
            Debug.LogError("LinkedList is empty!");
            return;
        }

        if (currentNode != null)
        {
            agent.SetDestination(currentNode.waypoint.position);
        }
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            MoveToNextWaypoint();
        }
    }

    void MoveToNextWaypoint()
    {
        if (currentNode == null) return;

        currentNode = currentNode.next;
        agent.SetDestination(currentNode.waypoint.position);
    }
}
