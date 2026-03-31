using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    //check when to fetch dialogue from queue and display it
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            DialogueManager.Instance.PlayNextSegment();
            hasTriggered = true;
        }
    }
}