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

    public DialogueDatabase fullLevelDatabase;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Fill the queue once at the start of the level
        if (fullLevelDatabase != null)
        {
            foreach (DialogueEntry entry in fullLevelDatabase.encounterDialogues)
            {
                dialogueQueue.Enqueue(entry);
            }
        }
    }

    // This is what the Trigger will now call
    public void PlayNextSegment()
    {
        // Only play if we aren't already talking and there is something left
        if (!currentlyPlaying && dialogueQueue.Count > 0)
        {
            StartCoroutine(ProcessSingleDialogue());
        }
    }

    private IEnumerator ProcessSingleDialogue()
    {
        currentlyPlaying = true;

        // Drone flies in
        yield return StartCoroutine(drone.ShowDrone());

        // Pull exactly ONE entry from the queue
        DialogueEntry currentDialogue = dialogueQueue.Dequeue();
        if (currentDialogue != null)
        {
            drone.DisplayText(currentDialogue.dialogueText);
            yield return new WaitForSeconds(currentDialogue.displayDuration);
        }

        // Drone flies away immediately after that one line
        yield return StartCoroutine(drone.HideDrone());

        currentlyPlaying = false;
    }

    // Start dialogue using your database
    /*public void StartDialogue(DialogueDatabase database)
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

    /*private IEnumerator ProcessDialogue()
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
    }*/

    private void DisplayDialogue(DialogueEntry entry)
    {
        Debug.Log($"{entry.speakerName}: {entry.dialogueText}");

        // Later connect to UI here
    }
}