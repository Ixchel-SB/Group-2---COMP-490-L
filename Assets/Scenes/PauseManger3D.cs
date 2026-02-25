using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager3D : MonoBehaviour
{
    public GameObject pauseMenu3D;        // drag PauseMenu3D here
    public Transform playerHead;          // drag Main Camera here
    public MonoBehaviour playerController; // drag ThirdPersonController or PlayerArmature here
    public float distanceInFront = 2f;   // distance of menu from player

    private bool isPaused = false;

    void Start()
    {
        Debug.Log("PauseManager3D Start on scene: " + SceneManager.GetActiveScene().name);
        pauseMenu3D.SetActive(false);
    }

    void OnDestroy()
    {
        Debug.Log("PauseManager3D being destroyed");
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Only process input if in game scene
        if (SceneManager.GetActiveScene().name != "SampleScene" && 
            SceneManager.GetActiveScene().name != "StoryTextScene")
        {
            return;
        }

        // Press P to pause
        if (Keyboard.current.pKey.wasPressedThisFrame && !isPaused)
        {
            Debug.Log("P key pressed - pausing");
            Pause();
        }
        // Press P again to resume
        else if (Keyboard.current.pKey.wasPressedThisFrame && isPaused)
        {
            Debug.Log("P key pressed - resuming");
            Resume();
        }
    }

    void Pause()
    {
        Time.timeScale = 0f;
        playerController.enabled = false;

        pauseMenu3D.SetActive(true);

        // Position menu in front of the player
        pauseMenu3D.transform.position = playerHead.position + playerHead.forward * distanceInFront;

        // Rotate menu to face the player
        pauseMenu3D.transform.LookAt(playerHead);
        pauseMenu3D.transform.Rotate(0, 180f, 0);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
        Debug.Log("Game paused - time scale: " + Time.timeScale);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        playerController.enabled = true;

        pauseMenu3D.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
        Debug.Log("Game resumed - time scale: " + Time.timeScale);
    }
}

