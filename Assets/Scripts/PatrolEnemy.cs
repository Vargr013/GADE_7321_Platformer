using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemy : AIEnemyBase
{
    private NavMeshAgent agent;

    private enum State { Patrol, Chase }
    private State currentState = State.Patrol;

    private WaypointNode currentNode;
    public NavigationLinkedList waypointList;

    public float loseSightTime = 3f;
    private float loseTimer = 0f;
    private Vector3 lastKnownPosition;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        currentNode = waypointList.GetHead();

        // Start patrolling
        if (currentNode != null)
        {
            agent.SetDestination(currentNode.waypoint.position);
        }
    }

    void Update()
    {
        if (player == null) return;

        bool canSeePlayer = CanSeePlayer();

        // State transitions
        if (canSeePlayer)
        {
            currentState = State.Chase;
            loseTimer = 0f;

            // Remember last seen position
            lastKnownPosition = player.position;
        }
        else
        {
            if (currentState == State.Chase)
            {
                loseTimer += Time.deltaTime;

                if (loseTimer >= loseSightTime)
                {
                    currentState = State.Patrol;
                }
            }
        }

        // State actions
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                Chase();
                break;
        }
    }

    // Detection with line of sight
    bool CanSeePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
            return false;

        RaycastHit hit;

        // Cast ray from enemy's head to player's head
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 target = player.position + Vector3.up * 1f;
        Vector3 direction = (target - origin).normalized;

        Debug.DrawRay(origin, direction * detectionRange, Color.red);

        // Check if ray hits the player
        if (Physics.Raycast(origin, direction, out hit, detectionRange))
        {
            if (hit.transform.root == player)
                return true;
        }

        return false;
    }


    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            MoveToNextWaypoint();
        }
    }

    void Chase()
    {
        // Continuously update destination to player's current position
        agent.SetDestination(player.position);
    }

    void MoveToNextWaypoint()
    {
        // Move to the next waypoint in the linked list
        if (currentNode == null) return;

        currentNode = currentNode.next;
        agent.SetDestination(currentNode.waypoint.position);
    }

    public override void Act()
    {
        // Patrol logic already handled by patrol script
    }
}
