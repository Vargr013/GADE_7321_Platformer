using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Static allows easy access
    public static GameManager Instance { get; private set; }

    [Header("Global Stats")]
    public int currentLives = 3;
    public int currentScore = 0;

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
        //Get Player in scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        //Validation
        if (player != null)
        {
            // Save the starting position as the first checkpoint
            SaveCheckpoint(player.transform.position);
        }
    }

    // Called whenever the player hits a checkpoint trigger
    public void SaveCheckpoint(Vector3 position)
    {
        // Pop(Delete) the old checkpoint if a new one is reached
        if (!checkpointStack.IsEmpty())
        {
            checkpointStack.Pop();
        }

        // Create a new data snapshot and push it onto our custom stack
        CheckPointData newData = new CheckPointData(position, currentLives, currentScore);
        checkpointStack.Push(newData);
        
        Debug.Log($"Checkpoint Saved at {position}. Score: {currentScore}");
    }

    // Handles player death and respawning
    public void RespawnPlayer(GameObject player)
    {
        //Validation
        if (checkpointStack.IsEmpty()) return;

        currentLives--; // Reduce lives by 1 on death

        //Use Peek to find where to spawn
        CheckPointData lastPoint = checkpointStack.Peek();
        
        // Update the lives count inside the stored checkpoint data
        lastPoint.savedLives = currentLives;

        //Check if player is still alive after losing a live
        if (currentLives > 0)
        {
            // Reset player position to the last checkpoint
            player.transform.position = lastPoint.checkpointPosition;
            Debug.Log($"Player Respawned. Lives remaining: {currentLives}");
        }
        else
        //Player has run out of lives
        {
            // Handle the end of the game
            Debug.Log("Game Over: Out of lives!");
        }
    }
    
    // Helper method to increase score from coins/enemies
    public void AddScore(int amount) => currentScore += amount;
}