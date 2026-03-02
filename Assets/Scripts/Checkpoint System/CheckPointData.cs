[System.Serializable] 
public class CheckPointData
{
    public UnityEngine.Vector3 checkpointPosition; // Where the player should respawn
    public int savedLives;                         // How many lives the player had at the time
    public int savedScore;                         // The score the player had at the time

    // Constructor to easily create a new checkpoint data instance
    public CheckPointData(UnityEngine.Vector3 incomingPos, int lives, int score)
    {
        checkpointPosition = incomingPos;
        savedLives = lives;
        savedScore = score;
    }
}