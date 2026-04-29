using UnityEngine;

public class WaypointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            AIEnemyPatrol patrol = other.GetComponent<AIEnemyPatrol>();
            if (patrol != null)
            {
                patrol.SendMessage("MoveToNextWaypoint");
            }
        }
    }
}
