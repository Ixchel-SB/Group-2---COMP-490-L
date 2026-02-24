using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuQuit : MonoBehaviour
{
    // Call this from your PauseMenu Quit button
    public void QuitToMainMenu()
    {
        Debug.Log("Returning to Main Menu...");

        // Make sure game is unpaused
        Time.timeScale = 1f;

        // Load MainMenu scene (replace with your actual scene name)
        SceneManager.LoadScene("MainMenu");
    }
}