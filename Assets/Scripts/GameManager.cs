using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    void Start()
    {
        // Get Player in scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Validation
        if (player != null)
        {
            // Grab the PlayerStats component from the player object
            playerStats = player.GetComponent<PlayerStats>();

            if (playerStats == null)
            {
                Debug.LogError("PlayerStats component not found on Player object!");
                return;
            }

            // Save the starting position as the first checkpoint
            SaveCheckpoint(player.transform.position);
        }
    }

    // Called whenever the player hits a checkpoint trigger
    public void SaveCheckpoint(Vector3 position)
    {
        // Pop (delete) the old checkpoint if a new one is reached
        if (!checkpointStack.IsEmpty())
        {
            checkpointStack.Pop();
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
            controller.enabled = true;
        }
        else
        {
            player.transform.position = lastPoint.checkpointPosition;
        }

        Debug.Log($"Respawned {player.name} to {lastPoint.checkpointPosition}");
    }

    // Convenience passthrough so other scripts can add score via GameManager
    public void AddScore(int amount) => playerStats.AddScore(amount);
}