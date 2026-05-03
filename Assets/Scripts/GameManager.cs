using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string DefaultSpawnId = "Default";

    // Static allows easy access
    public static GameManager Instance { get; private set; }

    // Reference to the PlayerStats component
    private PlayerStats playerStats;

    // Instance of our custom stack, told to hold CheckPointData objects (Replacing generic)
    private CustomStack<CheckPointData> checkpointStack = new CustomStack<CheckPointData>();

    private void Awake()
    {
        // Singleton: If an instance already exists, destroy this one.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    void Start()
    {
        RefreshPlayerContext();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshPlayerContext();
    }

    private void RefreshPlayerContext()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }

        playerStats = player.GetComponent<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component not found on Player object!");
            return;
        }

        MovePlayerToSceneSpawn(player);

        if (PlayerProgress.Instance != null)
        {
            PlayerProgress.Instance.BindToScenePlayer();
        }

        SaveCheckpoint(player.transform.position);
    }

    public void LoadLevelFromExit(GameObject player, string targetSceneName)
    {
        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogWarning("Cannot load level because the target scene name is empty.");
            return;
        }

        PlayerStats exitingStats = player != null ? player.GetComponent<PlayerStats>() : playerStats;
        if (PlayerProgress.Instance != null)
        {
            PlayerProgress.Instance.CaptureStats(exitingStats);
        }

        SceneManager.LoadScene(targetSceneName);
    }

    private void MovePlayerToSceneSpawn(GameObject player)
    {
        LevelSpawnPoint spawnPoint = FindSceneSpawnPoint(DefaultSpawnId);
        if (spawnPoint == null)
        {
            return;
        }

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        player.transform.SetPositionAndRotation(spawnPoint.transform.position, spawnPoint.transform.rotation);

        if (controller != null)
        {
            controller.enabled = true;
        }
    }

    private LevelSpawnPoint FindSceneSpawnPoint(string spawnId)
    {
        LevelSpawnPoint[] spawnPoints = FindObjectsByType<LevelSpawnPoint>(FindObjectsSortMode.None);
        foreach (LevelSpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.SpawnId == spawnId)
            {
                return spawnPoint;
            }
        }

        return spawnPoints.Length > 0 ? spawnPoints[0] : null;
    }

    // Called whenever the player hits a checkpoint trigger
    public void SaveCheckpoint(Vector3 position)
    {
        // Pop (delete) the old checkpoint if a new one is reached
        if (!checkpointStack.IsEmpty())
        {
            checkpointStack.Pop();
        }

        if (playerStats == null)
        {
            Debug.LogWarning("Cannot save checkpoint because no PlayerStats reference is available.");
            return;
        }

        // Create a new data snapshot and push it onto our custom stack
        CheckPointData newData = new CheckPointData(position, playerStats.currentLives, playerStats.currentScore);
        checkpointStack.Push(newData);

        Debug.Log($"Checkpoint Saved at {position}. Score: {playerStats.currentScore}");
    }

    // Handles player death and respawning
    public void RespawnPlayer(GameObject player)
    {
        if (checkpointStack.IsEmpty())
        {
            Debug.LogWarning("No checkpoint stored.");
            return;
        }

        CheckPointData lastPoint = checkpointStack.Peek();

        CharacterController controller = player.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = lastPoint.checkpointPosition;
            PlayerStats currentPlayerStats = player.GetComponent<PlayerStats>();
            if (currentPlayerStats == null)
            {
                Debug.LogError("PlayerStats component not found on Player object!");
                controller.enabled = true;
                return;
            }

            //Remove life on repsawn
            currentPlayerStats.LoseLife();
            if (!currentPlayerStats.IsAlive())
            {
                Debug.Log("Game Over!");
                // Will Implement game over logic here 
                //Pause game
                Time.timeScale = 0f;
            }
            controller.enabled = true;
        }
        else
        {
            player.transform.position = lastPoint.checkpointPosition;
        }

        Debug.Log($"Respawned {player.name} to {lastPoint.checkpointPosition}");
    }
}
