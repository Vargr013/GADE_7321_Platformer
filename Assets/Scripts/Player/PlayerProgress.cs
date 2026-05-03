using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance { get; private set; }

    public PlayerStats ActivePlayerStats { get; private set; }
    public int CarriedLives { get; private set; }
    public int CarriedScore { get; private set; }

    public event Action<PlayerStats> ActivePlayerChanged;

    private bool hasCarriedStats;

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

    private PlayerStats FindScenePlayerStats()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        return player != null ? player.GetComponent<PlayerStats>() : null;
    }

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
