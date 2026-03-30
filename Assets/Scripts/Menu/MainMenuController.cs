using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Scene names can be set in the inspector for flexibility
    [Header("Scene Names")]
    [SerializeField] private string Level1SceneName = "Level_1";


    // Methods to be called by UI buttons
    //PlayGame loads the specified play scene. 
    public void PlayGame()
    {
        Debug.Log("Play Game fired.");
        SceneManager.LoadScene(Level1SceneName);
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