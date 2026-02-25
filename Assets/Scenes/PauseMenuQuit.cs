using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuQuit : MonoBehaviour
{
    public PauseManager3D pauseManager;   // drag PauseManager3D here

    public void QuitToMainMenu()
    {
        Debug.Log("Returning to Main Menu...");

        // Force time to normal
        Time.timeScale = 1f;

        // Reset cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Destroy pause manager
        if (pauseManager != null)
        {
            pauseManager.Resume();
            Destroy(pauseManager.gameObject);
        }

        // Load main menu
        SceneManager.LoadScene("Scenes/MainMenu");

        // FIX THE EDITOR VIEW AUTOMATICALLY
        #if UNITY_EDITOR
        // Wait a tiny moment for the scene to load, then unpause and switch views
        UnityEditor.EditorApplication.delayCall += () => {
            // First unpause the editor
            UnityEditor.EditorApplication.isPaused = false;
            Debug.Log("Editor unpaused: " + !UnityEditor.EditorApplication.isPaused);
            
            // Then switch to Game view
            UnityEditor.EditorApplication.ExecuteMenuItem("Window/General/Game");
            
            // Get the game view and make sure it's visible
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            UnityEditor.EditorWindow.GetWindow(T, false, "Game", true);
            
            // Repaint all views
            UnityEditor.EditorApplication.RepaintHierarchyWindow();
            UnityEditor.EditorApplication.RepaintProjectWindow();
            
            Debug.Log("Editor should now be in Game view with pause OFF");
        };
        #endif
    }
}
