using UnityEngine;

// This script is attached to empty GameObjects in the scene that serve as spawn points for the player. Each spawn point has a unique identifier (spawnId) that can be used by the GameManager to determine where to respawn the player after death or when transitioning between levels. The spawnId can be set in the Unity Inspector for each spawn point. 
public class LevelSpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnId = "Default";

    public string SpawnId => spawnId;
}
