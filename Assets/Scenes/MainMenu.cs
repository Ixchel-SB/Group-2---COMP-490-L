using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("=== MAIN MENU AWAKE ===");
        Debug.Log("Time scale in Awake: " + Time.timeScale);
        Debug.Log("Cursor state in Awake: visible=" + Cursor.visible + ", lockState=" + Cursor.lockState);
    }

    void Start()
    {
        Debug.Log("=== MAIN MENU START ===");
        Debug.Log("Time scale in Start: " + Time.timeScale);
        
        Time.timeScale = 1f;
        Debug.Log("Time scale after reset: " + Time.timeScale);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Cursor state after reset: visible=" + Cursor.visible + ", lockState=" + Cursor.lockState);
        
        // List all active game objects in the scene
        Debug.Log("Active GameObjects in Main Menu:");
        foreach (var go in FindObjectsOfType<GameObject>())
        {
            if (go.activeInHierarchy)
                Debug.Log(" - " + go.name + " (Scene: " + go.scene.name + ")");
        }
    }

    void OnEnable()
    {
        Debug.Log("=== MAIN MENU ON ENABLE ===");
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartNewGame()
    {
        Debug.Log("Start New Game clicked - loading Scenes/StoryTextScene");
        SceneManager.LoadScene("Scenes/StoryTextScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game clicked");
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
