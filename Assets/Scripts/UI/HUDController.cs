using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    // Editor-assigned references for UI elements and player stats
    [Header("Player Reference")]
    public PlayerStats playerStats;

    [Header("TextMeshPro UI")]
    public TMP_Text scoreText;
    public TMP_Text livesText;

    [Header("Life Icons")]
    public Image[] lifeIcons;

    private int lastScore = int.MinValue;
    private int lastLives = int.MinValue;
    private PlayerStats subscribedStats;

    //Upon Awake, ensure we have the correct life icons assigned based on children named "Heart"
    private void Awake()
    {
        EnsureLifeIcons();
    }

    //Upon enabling, subscribe to scene load and player change events to update HUD accordingly
    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        if (PlayerProgress.Instance != null) PlayerProgress.Instance.ActivePlayerChanged += Bind;
    }

    //Upon start, bind to the current player stats to initialize the HUD display
    private void Start()
    {
        Bind(GetCurrentPlayerStats());
    }

    //Upon disabling, unsubscribe from events to prevent memory leaks and unintended behavior
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        if (PlayerProgress.Instance != null) PlayerProgress.Instance.ActivePlayerChanged -= Bind;
        Unsubscribe();
    }

    //When a new scene is loaded, attempt to bind to the current player stats to update the HUD for the new scene
    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Bind(GetCurrentPlayerStats());
    }

    //Bind to a new PlayerStats instance, unsubscribing from the old one if necessary, and refresh the HUD display
    private void Bind(PlayerStats stats)
    {
        if (stats == subscribedStats) { Refresh(true); return; }

        Unsubscribe();
        playerStats = stats;
        subscribedStats = playerStats;
        if (subscribedStats != null) subscribedStats.OnStatsChanged += RefreshFromStatsEvent;
        Refresh(true);
    }

    // Helper method to find the current player stats, either from the PlayerProgress singleton or by finding the player GameObject in the scene
    private PlayerStats GetCurrentPlayerStats()
    {
        if (PlayerProgress.Instance != null && PlayerProgress.Instance.ActivePlayerStats != null)
        {
            return PlayerProgress.Instance.ActivePlayerStats;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        return player != null ? player.GetComponent<PlayerStats>() : playerStats;
    }

    // Unsubscribe from the current PlayerStats events to prevent memory leaks and unintended behavior when switching players or scenes
    private void Unsubscribe()
    {
        if (subscribedStats == null) return;
        subscribedStats.OnStatsChanged -= RefreshFromStatsEvent;
        subscribedStats = null;
    }

    // Refresh the HUD display based on the current player stats, with an option to force refresh even if values haven't changed
    private void RefreshFromStatsEvent()
    {
        Refresh(false);
    }

    // Refresh the HUD display based on the current player stats, with an option to force refresh even if values haven't changed
    private void Refresh(bool forceRefresh)
    {
        if (playerStats == null) return;
        if (!forceRefresh && lastScore == playerStats.currentScore && lastLives == playerStats.currentLives) return;

        lastScore = playerStats.currentScore;
        lastLives = playerStats.currentLives;

        if (scoreText != null) scoreText.text = lastScore.ToString();
        if (livesText != null) livesText.text = lastLives.ToString();
        UpdateLifeIcons(lastLives);
    }

    // Update the life icons based on the current number of lives, ensuring we have the correct icons assigned and active
    private void UpdateLifeIcons(int lives)
    {
        EnsureLifeIcons();
        if (lifeIcons == null) return;

        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (lifeIcons[i] != null) lifeIcons[i].gameObject.SetActive(i < lives);
        }
    }

    // Ensure we have the correct life icons assigned based on children named "Heart", refreshing the array if any icons are missing or null
    private void EnsureLifeIcons()
    {
        bool needsRefresh = lifeIcons == null || lifeIcons.Length == 0;
        if (!needsRefresh)
        {
            foreach (Image icon in lifeIcons)
            {
                if (icon == null) { needsRefresh = true; break; }
            }
        }

        if (!needsRefresh) return;

        List<Image> icons = new List<Image>();
        foreach (Image image in GetComponentsInChildren<Image>(true))
        {
            if (image.gameObject.name.StartsWith("Heart", StringComparison.OrdinalIgnoreCase))
            {
                icons.Add(image);
            }
        }

        icons.Sort((a, b) => string.CompareOrdinal(a.gameObject.name, b.gameObject.name));
        lifeIcons = icons.ToArray();
    }
}
