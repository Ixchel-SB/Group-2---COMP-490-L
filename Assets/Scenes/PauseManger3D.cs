using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager3D : MonoBehaviour
{
    public GameObject pauseMenu3D;        // drag PauseMenu3D here
    public GameObject optionsCanvas;       // drag OptionsCanvas here
    public Transform playerHead;          // drag Main Camera here
    public MonoBehaviour playerController; // drag ThirdPersonController or PlayerArmature here
    public float distanceInFront = 2f;   // distance of menu from player
    public InventoryFramework.InventoryTester inventoryToggle; //drag PlayerArmature (has Inventory Tester) 

    private bool isPaused = false;
    private bool isOptionsActive = false;

    void Start()
    {
        Debug.Log("PauseManager3D Start on scene: " + SceneManager.GetActiveScene().name);
        pauseMenu3D.SetActive(false);
        if (optionsCanvas != null)
            optionsCanvas.SetActive(false);
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

        // Press P to pause (only if not in options)
        if (Keyboard.current.pKey.wasPressedThisFrame && !isPaused && !isOptionsActive)
        {
            Debug.Log("P key pressed - pausing");
            Pause();
        }
        // Press P again to resume (only if not in options)
        else if (Keyboard.current.pKey.wasPressedThisFrame && isPaused && !isOptionsActive)
        {
            Debug.Log("P key pressed - resuming");
            Resume();
        }
    }

    void Pause()
    {
        if (inventoryToggle != null)
            inventoryToggle.CloseInventory();

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
        isOptionsActive = false;
        Debug.Log("Game paused - time scale: " + Time.timeScale);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        playerController.enabled = true;

        pauseMenu3D.SetActive(false);
        if (optionsCanvas != null)
            optionsCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
        isOptionsActive = false;
        Debug.Log("Game resumed - time scale: " + Time.timeScale);
    }

    // Show options canvas (called by OptionsButton)
    public void ShowOptions()
    {
        Debug.Log("ShowOptions called");
        isOptionsActive = true;
        
        // Find the Canvas that contains all the UI elements
        Transform canvasTransform = pauseMenu3D.transform.Find("Canvas");
        if (canvasTransform != null)
        {
            // Hide all main menu UI elements (NOT the whole pauseMenu3D)
            Transform resumeBtn = canvasTransform.Find("ResumeButton");
            if (resumeBtn != null) resumeBtn.gameObject.SetActive(false);
            
            Transform optionsBtn = canvasTransform.Find("OptionsButton");
            if (optionsBtn != null) optionsBtn.gameObject.SetActive(false);
            
            Transform quitBtn = canvasTransform.Find("QuitButton");
            if (quitBtn != null) quitBtn.gameObject.SetActive(false);
            
            Transform pausedText = canvasTransform.Find("PausedText");
            if (pausedText != null) pausedText.gameObject.SetActive(false);
            
            Transform keys = canvasTransform.Find("Keys");
            if (keys != null) keys.gameObject.SetActive(false);
        }
        
        // Show options canvas
        if (optionsCanvas != null)
        {
            optionsCanvas.SetActive(true);
            Debug.Log("Options canvas shown");
        }
        else
        {
            Debug.LogError("OptionsCanvas is not assigned in PauseManager3D!");
        }
    }

    // Hide options canvas (called by BackButton)
    public void HideOptions()
    {
        Debug.Log("HideOptions called");
        isOptionsActive = false;
        
        // Find the Canvas that contains all the UI elements
        Transform canvasTransform = pauseMenu3D.transform.Find("Canvas");
        if (canvasTransform != null)
        {
            // Show all main menu UI elements again
            Transform resumeBtn = canvasTransform.Find("ResumeButton");
            if (resumeBtn != null) resumeBtn.gameObject.SetActive(true);
            
            Transform optionsBtn = canvasTransform.Find("OptionsButton");
            if (optionsBtn != null) optionsBtn.gameObject.SetActive(true);
            
            Transform quitBtn = canvasTransform.Find("QuitButton");
            if (quitBtn != null) quitBtn.gameObject.SetActive(true);
            
            Transform pausedText = canvasTransform.Find("PausedText");
            if (pausedText != null) pausedText.gameObject.SetActive(true);
            
            Transform keys = canvasTransform.Find("Keys");
            if (keys != null) keys.gameObject.SetActive(true);
        }
        
        // Hide options canvas
        if (optionsCanvas != null)
        {
            optionsCanvas.SetActive(false);
            Debug.Log("Options canvas hidden");
        }
    }
}

