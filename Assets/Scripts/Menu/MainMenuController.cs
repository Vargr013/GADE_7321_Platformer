using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    // Methods to be called by UI buttons
    //PlayGame loads the specified play scene. 
    public void PlayGame()
    {
        ResetProgressAndLoad(0);
    }

    public void LoadLevel1()
    {
        ResetProgressAndLoad(0);
    }

    public void LoadLevel2()
    {
        ResetProgressAndLoad(1);
    }

    public void LoadLevel3()
    {
        ResetProgressAndLoad(2);
    }

    private void ResetProgressAndLoad(int levelIndex)
    {
        if (PlayerProgress.Instance != null)
            PlayerProgress.Instance.ResetProgress();
        GameManager.Instance.LoadLevel(levelIndex);
    }


    // QuitGame exits the application and also stops play mode in the Unity Editor for testing purposes.
    public void QuitGame()
    {
        Debug.Log("Quit Game fired.");

        Application.Quit();

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}