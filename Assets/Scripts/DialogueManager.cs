using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    //make singleton for easy access
    public static DialogueManager Instance;

    //store the dialogue in queue
    private Queue<DialogueEntry> dialogueQueue = new Queue<DialogueEntry>();

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

        yield return StartCoroutine(drone.ShowDrone(null));

        //goes through the queue and displays dialogue one by one
        while (dialogueQueue.Count > 0)
        {
            //removes next dialogue from queue and displays it
            DialogueEntry currentDialogue = dialogueQueue.Dequeue();

            DisplayDialogue(currentDialogue);

            yield return new WaitForSeconds(currentDialogue.displayDuration);
        }

        drone.EndDialogue();
        currentlyPlaying = false;
    }

    private void DisplayDialogue(DialogueEntry entry)
    {
        Debug.Log($"{entry.speakerName}: {entry.dialogueText}");

        // Later connect to UI here
    }
}