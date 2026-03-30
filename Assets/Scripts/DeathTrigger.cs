using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Only trigger if the object hitting the checkpoint is the Player
        if (other.CompareTag("Player") )
        {
            // Tell the GameManager to Respawn and do the required logic to reset the player to the last checkpoint, as well as reducing lives
            GameManager.Instance.RespawnPlayer(other.gameObject);
        }
    }
}
