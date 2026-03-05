using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Entry")]
public class DialogueEntry : ScriptableObject
{
    public string speakerName;
    public Sprite icon;
    [TextArea(3, 10)]
    public string dialogueText;
    public float displayDuration = 3.0f; // How long it stays over the head
}