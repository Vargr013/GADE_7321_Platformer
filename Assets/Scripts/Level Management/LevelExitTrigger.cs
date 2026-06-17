using UnityEngine;

public class LevelExitTrigger : MonoBehaviour
{
    private bool hasTriggered;

    // This method is called when the player collider enters the trigger collider attached to the GameObject 
    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player"))
        {
            return;
        }

        // Set hasTriggered to true to prevent multiple triggers and call the GameManager to load the next level
        hasTriggered = true;
        GameManager.Instance.LoadNextLevel();
    }
}
