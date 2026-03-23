
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueDatabase", menuName = "Dialogue/Database")]
public class DialogueDatabase : ScriptableObject
{
    public DialogueEntry[] encounterDialogues;
}
