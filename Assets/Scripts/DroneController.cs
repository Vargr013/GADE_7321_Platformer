using System.Collections;
using UnityEngine;
using TMPro;

public class DroneController : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 2f, 2f); // In front of player

    public float moveSpeed = 3f;
    public float hoverHeight = 2f;

    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueCanvas;

    private Vector3 hiddenPosition;
    private bool isActive = false;

    void Start()
    {
        hiddenPosition = transform.position;
        dialogueCanvas.SetActive(false);
    }

    public IEnumerator ShowDrone(System.Action onComplete)
    {
        isActive = true;

        // Calculate position in front of player
        Vector3 targetPos = player.position + player.forward * offset.z + Vector3.up * offset.y;

        // Move up to player
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
            yield return null;
        }

        dialogueCanvas.SetActive(true);

        // Wait for dialogue system to finish
        yield return new WaitUntil(() => !isActive);

        // Move back down
        while (Vector3.Distance(transform.position, hiddenPosition) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, hiddenPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }

        dialogueCanvas.SetActive(false);

        onComplete?.Invoke();
    }

    public void DisplayText(string text)
    {
        dialogueText.text = text;
    }

    public void EndDialogue()
    {
        isActive = false;
    }

}
