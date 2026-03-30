using UnityEngine.InputSystem;
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

    private bool skipRequested = false;
    private bool continueDialogue = false;

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

    void Update()
    {
        if (currentlyPlaying && Keyboard.current.nKey.wasPressedThisFrame)
        {
            OnNextButtonPressed();
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

    public void OnNextButtonPressed()
    {
        skipRequested = true;
        continueDialogue = true;
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
            skipRequested = false;
            continueDialogue = false;

            drone.DisplayText(currentDialogue.dialogueText);
            drone.iconImage.sprite = currentDialogue.icon;

            float timer = 0f;

            // Wait for either timeout OR next button
            while (timer < currentDialogue.displayDuration && !skipRequested)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            // yield return new WaitForSeconds(currentDialogue.displayDuration);
        }

        // IF player pressed next → continue showing dialogue
        while (continueDialogue && dialogueQueue.Count > 0)
        {
            DialogueEntry nextDialogue = dialogueQueue.Dequeue();

            skipRequested = false;
            continueDialogue = false;

            drone.DisplayText(nextDialogue.dialogueText);
            drone.iconImage.sprite = nextDialogue.icon;

            float timer = 0f;

            while (timer < nextDialogue.displayDuration && !skipRequested)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        // If no next pressed → or finished → drone leaves
        yield return StartCoroutine(drone.HideDrone());

        currentlyPlaying = false;

        // Drone flies away immediately after that one line
        /*yield return StartCoroutine(drone.HideDrone());

        currentlyPlaying = false;*/
    }


    private void DisplayDialogue(DialogueEntry entry)
    {
        Debug.Log($"{entry.speakerName}: {entry.dialogueText}");

    }
}