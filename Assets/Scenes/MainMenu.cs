using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartNewGame()
    {
        Debug.Log("Starting new game...");
        SceneManager.LoadScene("StoryTextScene"); //SampleScene
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
