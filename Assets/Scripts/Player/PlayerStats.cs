using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public int currentLives = 3;
    public int currentScore = 0;

    // Helper method to increase score from coins/enemies
    public void AddScore(int amount) => currentScore += amount;

    // Reduce lives by 1 on death, returns remaining lives
    public int LoseLife()
    {
        currentLives--;
        return currentLives;
    }

    public bool IsAlive() => currentLives > 0;
}