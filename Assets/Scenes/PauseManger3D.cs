using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseManager3D : MonoBehaviour
{
    public GameObject pauseMenu3D;
    public Transform playerHead;
    public MonoBehaviour playerController;
    public float distanceInFront = 2f;
    public InventoryFramework.InventoryTester inventoryToggle;

    private bool isPaused = false;
    private bool isOptionsActive = false;
    private GameObject cachedOptionsCanvas;
    
    // Simple pause blocking
    private bool canPause = true;
    
    // Track if player is interacting
    private bool isInteracting = false;

    void Start()
    {
        Time.timeScale = 1f;
        Debug.Log("PauseManager3D Started");
        
        // Find and cache OptionsCanvas
        FindAndCacheOptionsCanvas();
        
        if (pauseMenu3D != null)
        {
            pauseMenu3D.SetActive(false);
            
            // Hide OptionsCanvas at start
            if (cachedOptionsCanvas != null)
                cachedOptionsCanvas.SetActive(false);
        }
    }

    void FindAndCacheOptionsCanvas()
    {
        // Try multiple ways to find OptionsCanvas
        
        // Method 1: Find by name
        cachedOptionsCanvas = GameObject.Find("OptionsCanvas");
        
        // Method 2: Find under pauseMenu3D -> Canvas
        if (cachedOptionsCanvas == null && pauseMenu3D != null)
        {
            Transform canvas = pauseMenu3D.transform.Find("Canvas");
            if (canvas != null)
            {
                Transform options = canvas.Find("OptionsCanvas");
                if (options != null)
                    cachedOptionsCanvas = options.gameObject;
            }
        }
        
        // Method 3: Find anywhere in scene using FindObjectOfType
        if (cachedOptionsCanvas == null)
        {
            Canvas[] allCanvases = FindObjectsOfType<Canvas>(true);
            foreach (Canvas cv in allCanvases)
            {
                if (cv.name == "OptionsCanvas")
                {
                    cachedOptionsCanvas = cv.gameObject;
                    break;
                }
            }
        }
        
        if (cachedOptionsCanvas != null)
            Debug.Log("OptionsCanvas found and cached!");
        else
            Debug.LogError("OptionsCanvas could not be found anywhere!");
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "SampleScene" && 
            SceneManager.GetActiveScene().name != "StoryTextScene")
        {
            return;
        }

        // Check if player is interacting with F key
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            isInteracting = true;
            Debug.Log("Player started interacting - pause blocked");
        }
        
        // Reset interaction when F key is released (or after a short delay)
        if (Keyboard.current.fKey.wasReleasedThisFrame)
        {
            // Add a small delay to ensure interaction is complete
            Invoke("ResetInteraction", 0.5f);
        }

        // Allow pause only if:
        // 1. Not already paused
        // 2. Not in options menu
        // 3. Can pause (not blocked by other systems)
        // 4. NOT currently interacting
        if (Keyboard.current.pKey.wasPressedThisFrame && !isPaused && !isOptionsActive && canPause && !isInteracting)
        {
            Pause();
        }
        
        // Force buttons to work even with Time.timeScale = 0
        if (isPaused && Input.GetMouseButtonDown(0))
        {
            ForceUIClick();
        }
    }
    
    void ResetInteraction()
    {
        isInteracting = false;
        Debug.Log("Player finished interacting - pause unblocked");
    }

    void Pause()
    {
        Debug.Log("Pause() called");
        
        if (inventoryToggle != null)
            inventoryToggle.CloseInventory();

        Time.timeScale = 0f;
        
        if (playerController != null)
            playerController.enabled = false;

        if (pauseMenu3D != null)
        {
            pauseMenu3D.SetActive(true);
            
            // Hide OptionsCanvas when pausing
            if (cachedOptionsCanvas != null)
                cachedOptionsCanvas.SetActive(false);
            
            if (playerHead != null)
            {
                pauseMenu3D.transform.position = playerHead.position + playerHead.forward * distanceInFront;
                pauseMenu3D.transform.LookAt(playerHead);
                pauseMenu3D.transform.Rotate(0, 180f, 0);
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
        isOptionsActive = false;
    }

    void ForceUIClick()
    {
        if (EventSystem.current == null) return;
        
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        
        foreach (var result in results)
        {
            Button button = result.gameObject.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                Debug.Log("Force clicking: " + button.name);
                button.onClick.Invoke();
                return;
            }
        }
    }

    public void Resume()
    {
        Debug.Log("!!! RESUME BUTTON WORKED !!!");
        Time.timeScale = 1f;
        
        if (playerController != null)
            playerController.enabled = true;

        if (pauseMenu3D != null)
            pauseMenu3D.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
        isOptionsActive = false;
        
        // Hide OptionsCanvas when resuming
        if (cachedOptionsCanvas != null)
            cachedOptionsCanvas.SetActive(false);
    }

    public void ShowOptions()
    {
        Debug.Log("ShowOptions() called");
        isOptionsActive = true;
        
        // Make sure we have the OptionsCanvas cached
        if (cachedOptionsCanvas == null)
            FindAndCacheOptionsCanvas();
        
        // Force show OptionsCanvas
        if (cachedOptionsCanvas != null)
        {
            cachedOptionsCanvas.SetActive(true);
            Debug.Log("OptionsCanvas forced to show!");
            
            // Make sure all children are active
            foreach (Transform child in cachedOptionsCanvas.transform)
            {
                child.gameObject.SetActive(true);
            }
            
            // Bring to front
            cachedOptionsCanvas.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogError("OptionsCanvas is null!");
        }
        
        // Hide the main menu buttons
        if (pauseMenu3D != null)
        {
            Button[] buttons = pauseMenu3D.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                bool isInOptions = false;
                Transform parent = btn.transform.parent;
                while (parent != null)
                {
                    if (parent.gameObject == cachedOptionsCanvas)
                    {
                        isInOptions = true;
                        break;
                    }
                    parent = parent.parent;
                }
                
                if (!isInOptions && btn.name != "BackButton")
                {
                    btn.gameObject.SetActive(false);
                }
            }
        }
    }

    public void HideOptions()
    {
        Debug.Log("HideOptions() called");
        isOptionsActive = false;
        
        // Hide OptionsCanvas
        if (cachedOptionsCanvas != null)
        {
            cachedOptionsCanvas.SetActive(false);
            Debug.Log("OptionsCanvas hidden");
        }
        
        // Show all buttons again
        if (pauseMenu3D != null)
        {
            Button[] buttons = pauseMenu3D.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                btn.gameObject.SetActive(true);
            }
        }
    }
    
    public void OnResumeButton()
    {
        Debug.Log("!!! RESUME BUTTON CLICK DETECTED !!!");
        Resume();
    }
    
    public void OnOptionsButton()
    {
        Debug.Log("!!! OPTIONS BUTTON CLICK DETECTED !!!");
        ShowOptions();
    }
    
    public void OnQuitButton()
    {
        Debug.Log("!!! QUIT BUTTON CLICK DETECTED !!!");
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }
    
    // Public methods for other scripts to block/unblock pause
    public void BlockPause()
    {
        canPause = false;
        Debug.Log("Pause blocked by external script");
    }
    
    public void UnblockPause()
    {
        canPause = true;
        Debug.Log("Pause unblocked by external script");
    }
    
    // Method to manually set interacting state (for other scripts to call)
    public void SetInteracting(bool interacting)
    {
        isInteracting = interacting;
        if (interacting)
            Debug.Log("Interaction started - pause blocked");
        else
            Debug.Log("Interaction ended - pause unblocked");
    }
    
    // Public method for other scripts to check if game is paused
    public bool IsGamePaused()
    {
        return isPaused;
    }
}

