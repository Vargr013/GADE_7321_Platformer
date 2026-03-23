using System.Collections;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    //make singleton for easy access
    public static DialogueManager Instance;

    //store the dialogue in custom queue
    private DialogueQueue dialogueQueue = new DialogueQueue(20);

    //checks if a dialogue is already playing
    private bool currentlyPlaying = false;

    public DroneController drone;

    private void Awake()
    {
        Instance = this;
    }

    // Start dialogue using your database
    public void StartDialogue(DialogueDatabase database)
    {
        //clears previous dialogue
        dialogueQueue.Clear();

        //adds dialogue to queue
        foreach (DialogueEntry entry in database.encounterDialogues)
        {
            dialogueQueue.Enqueue(entry); // FIFO
        }

        if (!currentlyPlaying)
        {
            StartCoroutine(ProcessDialogue());
        }
    }

    private IEnumerator ProcessDialogue()
    {
        currentlyPlaying = true;

        //StartCoroutine(drone.ShowDrone(null));
        yield return StartCoroutine(drone.ShowDrone());

        //goes through the queue and displays dialogue one by one
        while (dialogueQueue.Count > 0)
        {
            //removes next dialogue from queue and displays it
            DialogueEntry currentDialogue = dialogueQueue.Dequeue();

            drone.DisplayText(currentDialogue.dialogueText);

            yield return new WaitForSeconds(currentDialogue.displayDuration);
        }

        // Hide UI
        drone.dialogueCanvas.SetActive(false);

        // Move drone back
        yield return StartCoroutine(drone.HideDrone());

        //drone.EndDialogue();
        currentlyPlaying = false;
    }

    private void DisplayDialogue(DialogueEntry entry)
    {
        Debug.Log($"{entry.speakerName}: {entry.dialogueText}");

        // Later connect to UI here
    }
}