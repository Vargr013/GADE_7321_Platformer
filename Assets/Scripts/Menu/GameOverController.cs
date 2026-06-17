using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string level1SceneName = "Level_1_Beginner";

    public void RestartFromLevel1()
    {
        Time.timeScale = 1f;

        if (PlayerProgress.Instance != null)
        {
            PlayerProgress.Instance.ResetProgress();
        }

        SceneManager.LoadScene(level1SceneName);
    }

    public void RestartCurrentLevel()
    {
        Time.timeScale = 1f;

        if (PlayerProgress.Instance != null)
            PlayerProgress.Instance.ResetProgress();

        int levelIndex = GameManager.Instance != null ? GameManager.Instance.GetCurrentLevelIndex() : 0;
        GameManager.Instance.LoadLevel(levelIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game fired.");
        Application.Quit();

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}
