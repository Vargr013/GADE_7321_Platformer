using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    private bool hasBeenTriggered = false; // Prevents the player from re-saving at the same spot

    private void OnTriggerEnter(Collider other)
    {
        // Only trigger if the object hitting the checkpoint is the Player
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            // Tell the GameManager to save the current state at this trigger's position
            GameManager.Instance.SaveCheckpoint(transform.position);
            hasBeenTriggered = true; 
        }
    }
}