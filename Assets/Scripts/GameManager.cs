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
        // Validation
        if (checkpointStack.IsEmpty()) return;

        int remainingLives = playerStats.LoseLife(); // Delegate life loss to PlayerStats

        // Use Peek to find where to spawn
        CheckPointData lastPoint = checkpointStack.Peek();

        // Update the lives count inside the stored checkpoint data
        lastPoint.savedLives = remainingLives;

        // Check if player is still alive after losing a life
        if (playerStats.IsAlive())
        {
            // Reset player position to the last checkpoint
            player.transform.position = lastPoint.checkpointPosition;
            Debug.Log($"Player Respawned. Lives remaining: {remainingLives}");
        }
        else
        {
            // Handle the end of the game
            Debug.Log("Game Over: Out of lives!");
        }
    }

    // Convenience passthrough so other scripts can add score via GameManager
    public void AddScore(int amount) => playerStats.AddScore(amount);
}