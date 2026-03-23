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

    void Update()
    {
        if (player != null)
        {
            // Always face the player
            transform.LookAt(player.position + Vector3.up * (hoverHeight / 2));

            // If dialogue is active, maintain position relative to player
            if (isActive)
            {
                Vector3 targetPos = player.position + (player.forward * offset.z) + (Vector3.up * offset.y);
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            }
        }
    }

    public IEnumerator ShowDrone()
    {
        

        // Calculate position in front of player
        Vector3 targetPos = player.position + player.forward * offset.z + Vector3.up * offset.y;

        // Move up to player
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            // Make sure to remain consistent position in front of player 
            targetPos = player.position + (player.forward * offset.z) + (Vector3.up * offset.y);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        isActive = true;
        dialogueCanvas.SetActive(true);

        // Wait for dialogue system to finish
        //yield return new WaitUntil(() => !isActive);

        
        
    }

    // Move back down
    public IEnumerator HideDrone()
    {
        isActive = false;
        
        // Go back to original position
        while (Vector3.Distance(transform.position, hiddenPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, hiddenPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void DisplayText(string text)
    {
        dialogueText.text = text;
    }

    public void EndDialogue()
    {
        //isActive = false;
    }

}
