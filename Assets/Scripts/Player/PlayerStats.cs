using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public int currentLives = 3;
    public int currentScore = 0;

    // Called whenever score or lives changes
    public event Action OnStatsChanged;

    // Helper method to increase score from coins/enemies
    public void AddScore(int amount)
    {
        currentScore += amount;
        OnStatsChanged?.Invoke();
    }

    // Reduce lives by 1 on death, returns remaining lives
    public int LoseLife()
    {
        currentLives--;
        OnStatsChanged?.Invoke();
        return currentLives;
    }

    public bool IsAlive() => currentLives > 0;
}
