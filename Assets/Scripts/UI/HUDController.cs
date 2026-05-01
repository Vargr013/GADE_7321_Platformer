using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Player Reference")]
    // Optional reference. If empty, this script finds the Player by tag
    public PlayerStats playerStats;

    [Header("TextMeshPro UI")]
    // References to the TextMeshPro UI elements that display the score and lives.
    public TMP_Text scoreText;
    public TMP_Text livesText;

    // Labels that are used to display the score and lives in the UI.
    [Header("Labels")]
    public string scorePrefix = "Score: ";
    public string livesPrefix = "Lives: ";

    // Store the last values so the UI only updates when something changes
    private int lastScore = int.MinValue;
    private int lastLives = int.MinValue;

    private void Start()
    {
        FindPlayerStatsIfNeeded();
        SubscribeToPlayerStats();
        Refresh(true);
    }

    private void OnDestroy()
    {
        UnsubscribeFromPlayerStats();
    }

    private void FindPlayerStatsIfNeeded()
    {
        if (playerStats != null) return;

        // Find the player automatically if no reference was assigned
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
        }
    }

    private void SubscribeToPlayerStats()
    {
        if (playerStats == null) return;

        // Refresh the HUD only when score or lives changes
        playerStats.OnStatsChanged += RefreshFromStatsEvent;
    }

    private void UnsubscribeFromPlayerStats()
    {
        if (playerStats == null) return;
        //Unsubscribe from the event to prevent memory leaks and null reference errors when the player is destroyed
        playerStats.OnStatsChanged -= RefreshFromStatsEvent;
    }

    private void RefreshFromStatsEvent()
    {
        Refresh(false);
    }

    private void Refresh(bool forceRefresh)
    {
        if (playerStats == null) return;

        // Skip the UI update when score and lives are unchanged
        if (!forceRefresh && lastScore == playerStats.currentScore && lastLives == playerStats.currentLives)
        {
            return;
        }

        // Update the last values to the current values
        lastScore = playerStats.currentScore;
        lastLives = playerStats.currentLives;

        // Update the UI with the new values
        SetText(scoreText, scorePrefix + lastScore);
        SetText(livesText, livesPrefix + lastLives);
    }

    private void SetText(TMP_Text tmpText, string value)
    {
        // Update TextMeshPro
        if (tmpText != null)
        {
            tmpText.text = value;
        }

    }
}
