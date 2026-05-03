using UnityEngine;

public class LevelExitTrigger : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "Level_2_Advanced";

    private bool hasTriggered;

    // This method is called when the player collider enters the trigger collider attached to the GameObject 
    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player"))
        {
            return;
        }

        // Set hasTriggered to true to prevent multiple triggers and call the GameManager to load the target scene, passing the player GameObject for any necessary transition effects or data handling
        hasTriggered = true;
        GameManager.Instance.LoadLevelFromExit(other.gameObject, targetSceneName);
    }
}
