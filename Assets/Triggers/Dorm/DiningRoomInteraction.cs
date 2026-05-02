using UnityEngine;
using TMPro;
using System.Collections;

public class DiningRoomInteraction : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI thinkingText;
    public GameObject blackScreenPanel;
    
    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.F;
    public float interactionRange = 3f;
    
    [Header("Doors to Lock")]
    public GameObject dormDoor;
    public GameObject entranceDoor;
    private Collider dormDoorCollider;
    private Collider entranceDoorCollider;
    private MonoBehaviour dormDoorScript;
    private MonoBehaviour entranceDoorScript;
    private bool entranceDoorLocked = false;
    
    [Header("Character Models - OLD (to disable)")]
    public GameObject oldMarcelo;
    public GameObject oldElio;
    
    [Header("Character Models - NEW (to enable AFTER closet interaction)")]
    public GameObject newMarcelo;
    public GameObject newElio;
    public GameObject newValentina;
    public GameObject newSamael;
    
    [Header("Thinking Text for Door")]
    public string doorThinkingText = "Should hurry up before the food is gone";
    public float doorThinkingDuration = 2f;
    
    [Header("Story Text")]
    [TextArea(10, 15)]
    public string storyText = "Metzly and her roommates ate their dinner and decided to go to the cemetery tomorrow before school starts, hoping this would clear Metzly's mind. Once they all finished, Metzly and Valentina took a bath and got all their supplies ready for school while the boys procrastinated the whole day. The girls were able to go to bed early.";
    
    [Header("Time Text")]
    public string timeText = "Monday 6:15am";
    
    [Header("Thinking After Chair")]
    public string thinkingAfterText = "I should head to the cemetery... I'm sure everyone else headed there by now.";
    public float thinkingAfterDuration = 4f;
    
    private CanvasGroup thinkingCanvasGroup;
    private CanvasGroup blackCanvasGroup;
    private bool hasInteracted = false;
    private bool isSequenceRunning = false;
    private bool waitingForF = false;
    private bool playerInTrigger = false;
    private bool closetSequenceCompleted = false;
    private bool chairInteractionDone = false;
    private bool door1Interacted = false;
    
    private GameObject player;
    private MonoBehaviour playerController;
    private PostPhotoSequence postPhotoSequence;
    private GameObject door1Trigger;
    
    void Start()
    {
        Debug.Log("=== DINING ROOM INTERACTION START ===");
        
        // Find PostPhotoSequence to check when closet interaction completes
        postPhotoSequence = FindObjectOfType<PostPhotoSequence>();
        
        // Find player
        player = GameObject.FindGameObjectWithTag("Player");
        
        // Find Door 1 trigger (the girls room door with arrow)
        door1Trigger = GameObject.Find("GirlsRoomDoor"); // Adjust name as needed
        
        // Setup thinking text
        if (thinkingText != null)
        {
            thinkingCanvasGroup = thinkingText.GetComponent<CanvasGroup>();
            if (thinkingCanvasGroup == null)
                thinkingCanvasGroup = thinkingText.gameObject.AddComponent<CanvasGroup>();
            thinkingCanvasGroup.alpha = 0f;
            thinkingText.gameObject.SetActive(false);
        }
        
        // Setup black screen
        if (blackScreenPanel != null)
        {
            blackCanvasGroup = blackScreenPanel.GetComponent<CanvasGroup>();
            if (blackCanvasGroup == null)
                blackCanvasGroup = blackScreenPanel.AddComponent<CanvasGroup>();
            blackCanvasGroup.alpha = 0f;
        }
        
        // Setup doors
        if (dormDoor != null)
        {
            dormDoorCollider = dormDoor.GetComponent<Collider>();
            dormDoorScript = dormDoor.GetComponent<MonoBehaviour>();
        }
        
        if (entranceDoor != null)
        {
            entranceDoorCollider = entranceDoor.GetComponent<Collider>();
            entranceDoorScript = entranceDoor.GetComponent<MonoBehaviour>();
            // Lock Entrance(1) at start
            LockEntranceDoor();
        }
        
        // Initially disable new models
        if (newMarcelo != null) newMarcelo.SetActive(false);
        if (newElio != null) newElio.SetActive(false);
        if (newValentina != null) newValentina.SetActive(false);
        if (newSamael != null) newSamael.SetActive(false);
        
        // Make sure old models are enabled at start
        if (oldMarcelo != null) oldMarcelo.SetActive(true);
        if (oldElio != null) oldElio.SetActive(true);
        
        Debug.Log("=== DINING ROOM INTERACTION READY ===");
    }
    
    void LockEntranceDoor()
    {
        if (entranceDoor != null && !entranceDoorLocked)
        {
            if (entranceDoorCollider != null) entranceDoorCollider.enabled = false;
            if (entranceDoorScript != null) entranceDoorScript.enabled = false;
            entranceDoorLocked = true;
            Debug.Log("Entrance(1) LOCKED - Cannot interact before chair sequence");
        }
    }
    
    void UnlockEntranceDoor()
    {
        if (entranceDoor != null && entranceDoorLocked)
        {
            if (entranceDoorCollider != null) entranceDoorCollider.enabled = true;
            if (entranceDoorScript != null) entranceDoorScript.enabled = true;
            entranceDoorLocked = false;
            Debug.Log("Entrance(1) UNLOCKED - Can now interact");
        }
    }
    
    void Update()
    {
        // Check if closet sequence has completed
        if (!closetSequenceCompleted && postPhotoSequence != null)
        {
            if (postPhotoSequence.HasArrowPressed())
            {
                closetSequenceCompleted = true;
                Debug.Log("=== CLOSET SEQUENCE COMPLETED! Enabling new dining room models ===");
                EnableNewModels();
                DisableOldModels();
            }
        }
        
        // Check for Door 1 interaction (press F on the girls room door)
        if (!door1Interacted && closetSequenceCompleted && !chairInteractionDone && door1Trigger != null && player != null)
        {
            float distanceToDoor1 = Vector3.Distance(door1Trigger.transform.position, player.transform.position);
            if (distanceToDoor1 < 3f && Input.GetKeyDown(interactKey))
            {
                door1Interacted = true;
                Debug.Log("Door 1 interacted - showing thinking text");
                StartCoroutine(ShowDoorThinkingText());
            }
        }
        
        if (waitingForF && Input.GetKeyDown(KeyCode.F))
        {
            waitingForF = false;
        }
        
        // Only allow dining room interaction AFTER closet sequence is done, before chair interaction
        if (!hasInteracted && !isSequenceRunning && playerInTrigger && closetSequenceCompleted && !chairInteractionDone && Input.GetKeyDown(interactKey))
        {
            StartCoroutine(RunSequence());
        }
    }
    
    IEnumerator ShowDoorThinkingText()
    {
        if (thinkingText != null)
        {
            thinkingText.gameObject.SetActive(true);
            thinkingText.text = doorThinkingText;
            thinkingCanvasGroup.alpha = 1f;
            yield return new WaitForSeconds(doorThinkingDuration);
            
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                thinkingCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
                yield return null;
            }
            thinkingCanvasGroup.alpha = 0f;
            thinkingText.gameObject.SetActive(false);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            player = other.gameObject;
            playerController = player.GetComponent<MonoBehaviour>();
            
            if (closetSequenceCompleted && !chairInteractionDone)
            {
                Debug.Log("Player entered chair trigger area - Press F to sit at table");
                if (thinkingText != null)
                {
                    thinkingText.gameObject.SetActive(true);
                    thinkingText.text = "Press F to sit at the table";
                    thinkingCanvasGroup.alpha = 1f;
                }
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            player = null;
            
            if (thinkingText != null)
            {
                thinkingText.gameObject.SetActive(false);
            }
        }
    }
    
    void EnableNewModels()
    {
        Debug.Log("=== ENABLING NEW DINING ROOM MODELS ===");
        if (newMarcelo != null) newMarcelo.SetActive(true);
        if (newElio != null) newElio.SetActive(true);
        if (newValentina != null) newValentina.SetActive(true);
        if (newSamael != null) newSamael.SetActive(true);
    }
    
    void DisableOldModels()
    {
        Debug.Log("=== DISABLING OLD MODELS ===");
        if (oldMarcelo != null) oldMarcelo.SetActive(false);
        if (oldElio != null) oldElio.SetActive(false);
    }
    
    void DisableNewModelsExceptSamael()
    {
        Debug.Log("=== DISABLING NEW MODELS (Except Samael) ===");
        if (newMarcelo != null) newMarcelo.SetActive(false);
        if (newElio != null) newElio.SetActive(false);
        if (newValentina != null) newValentina.SetActive(false);
        // Samael stays enabled
    }
    
    void LockDoors()
    {
        if (dormDoor != null)
        {
            if (dormDoorCollider != null) dormDoorCollider.enabled = false;
            if (dormDoorScript != null) dormDoorScript.enabled = false;
        }
    }
    
    void UnlockDoors()
    {
        if (dormDoor != null)
        {
            if (dormDoorCollider != null) dormDoorCollider.enabled = true;
            if (dormDoorScript != null) dormDoorScript.enabled = true;
        }
    }
    
    void FreezePlayer()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    void UnfreezePlayer()
    {
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    IEnumerator RunSequence()
    {
        Debug.Log("=== DINING ROOM SEQUENCE STARTED ===");
        
        hasInteracted = true;
        isSequenceRunning = true;
        chairInteractionDone = true;
        
        // Hide the prompt
        if (thinkingText != null)
        {
            thinkingText.gameObject.SetActive(false);
        }
        
        LockDoors();
        FreezePlayer();
        
        // Fade to black
        if (blackCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                blackCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
                yield return null;
            }
            blackCanvasGroup.alpha = 1f;
        }
        
        // Show story text with wider text area
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = storyText;
        thinkingCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(8f);
        thinkingText.gameObject.SetActive(false);
        
        // Show time text
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = timeText;
        thinkingCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(3f);
        thinkingText.gameObject.SetActive(false);
        
        // Fade back from black
        if (blackCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                blackCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
                yield return null;
            }
            blackCanvasGroup.alpha = 0f;
        }
        
        // Show final thinking text
        yield return StartCoroutine(ShowThinkingText(thinkingAfterText, thinkingAfterDuration));
        
        // Disable new models except Samael
        DisableNewModelsExceptSamael();
        
        // Unlock Entrance(1) door after chair interaction
        UnlockEntranceDoor();
        
        UnlockDoors();
        UnfreezePlayer();
        isSequenceRunning = false;
        
        Debug.Log("=== DINING ROOM SEQUENCE COMPLETED ===");
    }
    
    IEnumerator ShowThinkingText(string message, float duration)
    {
        if (thinkingText == null) yield break;
        
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = message;
        
        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            thinkingCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
            yield return null;
        }
        thinkingCanvasGroup.alpha = 1f;
        
        yield return new WaitForSeconds(duration);
        
        elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            thinkingCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
            yield return null;
        }
        thinkingCanvasGroup.alpha = 0f;
        thinkingText.gameObject.SetActive(false);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
