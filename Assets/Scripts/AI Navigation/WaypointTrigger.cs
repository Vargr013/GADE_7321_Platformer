using UnityEngine;

public class WaypointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // If the enemy has an AIEnemyPatrol component, call MoveToNextWaypoint
            AIEnemyPatrol patrol = other.GetComponent<AIEnemyPatrol>();
            if (patrol != null)
            {
                patrol.SendMessage("MoveToNextWaypoint");
            }
        }
    }
}
