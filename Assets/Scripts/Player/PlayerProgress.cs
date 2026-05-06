using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerProgress : MonoBehaviour
{
    // Singleton instance to manage player progress across scenes, including lives and score
    public static PlayerProgress Instance { get; private set; }

    public PlayerStats ActivePlayerStats { get; private set; }
    public int CarriedLives { get; private set; }
    public int CarriedScore { get; private set; }

    public event Action<PlayerStats> ActivePlayerChanged;

    private bool hasCarriedStats;

    // Upon Awake, ensure this is the only instance of PlayerProgress and persist it across scene loads
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        BindToScenePlayer();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        UnsubscribeFromActivePlayer();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BindToScenePlayer();
    }

    // Attempt to find the player in the new scene and bind to their stats, carrying over lives and score if available
    public void BindToScenePlayer()
    {
        PlayerStats scenePlayerStats = FindScenePlayerStats();
        if (scenePlayerStats == null)
        {
            return;
        }

        UnsubscribeFromActivePlayer();
        ActivePlayerStats = scenePlayerStats;

        if (hasCarriedStats)
        {
            ActivePlayerStats.SetStats(CarriedLives, CarriedScore);
        }
        else
        {
            SnapshotActivePlayerStats();
            hasCarriedStats = true;
        }

        ActivePlayerStats.OnStatsChanged += HandleActivePlayerStatsChanged;
        ActivePlayerChanged?.Invoke(ActivePlayerStats);
    }

    // Capture the current stats from a PlayerStats instance to carry over to the next scene, such as when the player dies and respawns
    public void CaptureStats(PlayerStats stats)
    {
        if (stats == null)
        {
            return;
        }

        CarriedLives = stats.currentLives;
        CarriedScore = stats.currentScore;
        hasCarriedStats = true;
    }

    // Reset the carried stats to default values, such as when starting a new game or after a game over
    public void ResetProgress(int lives = 3, int score = 0)
    {
        CarriedLives = lives;
        CarriedScore = score;
        hasCarriedStats = true;
    }

    // Helper method to find the player GameObject in the scene and return their PlayerStats component, if it exists
    private PlayerStats FindScenePlayerStats()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        return player != null ? player.GetComponent<PlayerStats>() : null;
    }

    // Handle changes to the active player's stats by updating the carried stats to ensure they persist across scenes
    private void HandleActivePlayerStatsChanged()
    {
        SnapshotActivePlayerStats();
    }

    private void SnapshotActivePlayerStats()
    {
        if (ActivePlayerStats == null)
        {
            return;
        }

        CarriedLives = ActivePlayerStats.currentLives;
        CarriedScore = ActivePlayerStats.currentScore;
    }

    private void UnsubscribeFromActivePlayer()
    {
        if (ActivePlayerStats == null)
        {
            return;
        }

        ActivePlayerStats.OnStatsChanged -= HandleActivePlayerStatsChanged;
    }
}
