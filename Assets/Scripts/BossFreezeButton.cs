// BossFreezeButton.cs - Player-triggered switch that freezes all BossEnemy instances once every button in the scene has been pressed.

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BossFreezeButton : MonoBehaviour
{
    [Tooltip("If true, the button disables itself after being pressed once.")]
    public bool oneShot = true;

    // True after this specific button has been pressed. Public-read so the
    public bool HasFired { get; private set; }

    // Ensure the collider is a trigger so the player just walks into the radius.
    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HasFired) return;
        if (!other.CompareTag("Player")) return;
        if (other.GetComponentInParent<PlayerController>() == null) return;

        HasFired = true;
        if (oneShot) gameObject.SetActive(false);

        // Was this the last button? If so, freeze every boss in the scene and set the global flag so future waves also spawn frozen.
        if (AllButtonsPressed()) FreezeAllBosses();
    }

    // True if every BossFreezeButton currently in the scene reports HasFired.
    private bool AllButtonsPressed()
    {
        BossFreezeButton[] buttons = FindObjectsByType<BossFreezeButton>(FindObjectsSortMode.None);
        foreach (BossFreezeButton b in buttons) if (!b.HasFired) return false;
        return true;
    }

    // Centralised freeze logic: stops every current boss and locks future waves.
    private static void FreezeAllBosses()
    {
        BossEnemy[] bosses = FindObjectsByType<BossEnemy>(FindObjectsSortMode.None);
        foreach (BossEnemy b in bosses) b.Freeze();
        BossEnemy.GlobalFreeze = true;
    }
}
