using UnityEngine;
using TMPro;
using System.Collections;

public class DiningRoomInteraction : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI thinkingText;
    public GameObject blackScreenPanel;
    
    [Header("Dialogue UI Elements")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI continueText;
    
    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.F;
    public float interactionRange = 3f;
    
    [Header("Doors to Lock")]
    public GameObject dormDoor;
    public GameObject entranceDoor;
    public Transform outsideDormPosition;
    private Collider dormDoorCollider;
    private Collider entranceDoorCollider;
    private MonoBehaviour dormDoorScript;
    private MonoBehaviour entranceDoorScript;
    
    [Header("Character Models - OLD (to disable)")]
    public GameObject oldMarcelo;
    public GameObject oldElio;
    
    [Header("Character Models - NEW (to enable AFTER closet interaction)")]
    public GameObject newMarcelo;
    public GameObject newElio;
    public GameObject newValentina;
    public GameObject newSamael;
    public GameObject oldSamael; // The Samael model to remove after dialogue
    
    [Header("Thinking Texts")]
    public string doorThinkingText = "Should hurry up before the food is gone";
    public float doorThinkingDuration = 2f;
    
    public string hurryText = "Press F to sit at the table";
    public float hurryTextDuration = 2f;
    
    [Header("Story Text")]
    [TextArea(10, 20)]
    public string storyText = "Metzly and her roommates ate their dinner and decided to go to the cemetery tomorrow before school starts, hoping this would clear Metzly's mind. Once they all finished, Metzly and Valentina took a bath and got all their supplies ready for school while the boys procrastinated the whole day. The girls were able to go to bed early.";
    
    [Header("Time Text")]
    public string timeText = "Monday 6:15am";
    
    [Header("Thinking After")]
    public string thinkingAfterText = "I should head to the cemetery... I'm sure everyone else headed there by now.";
    public float thinkingAfterDuration = 4f;
    
    [Header("Samael Dialogue Lines")]
    public string[] samaelDialogueLines = new string[]
    {
        "HEY!",
        "Hey! Are you listening! *burp*",
        "The stupid nun sent me to wake you all up. So get up!",
        "*burp* *spits on floor*",
        "Good for you. *burp*"
    };
    
    public string[] metzlyDialogueLines = new string[]
    {
        "*Gasp*",
        "Yes",
        "Umm... We're already woke up."
    };
    
    private CanvasGroup thinkingCanvasGroup;
    private CanvasGroup blackCanvasGroup;
    private bool hasInteracted = false;
    private bool isSequenceRunning = false;
    private bool waitingForF = false;
    private bool playerInTrigger = false;
    private bool closetSequenceCompleted = false;
    private bool chairInteractionDone = false;
    private bool samaelDialogueDone = false;
    
    private GameObject player;
    private MonoBehaviour playerController;
    private PostPhotoSequence postPhotoSequence;
    
    void Start()
    {
        Debug.Log("=== DINING ROOM INTERACTION START ===");
        
        // Find PostPhotoSequence to check when closet interaction completes
        postPhotoSequence = FindObjectOfType<PostPhotoSequence>();
        
        // Find player
        player = GameObject.FindGameObjectWithTag("Player");
        
        // Setup thinking text
        if (thinkingText != null)
        {
            thinkingCanvasGroup = thinkingText.GetComponent<CanvasGroup>();
            if (thinkingCanvasGroup == null)
                thinkingCanvasGroup = thinkingText.gameObject.AddComponent<CanvasGroup>();
            thinkingCanvasGroup.alpha = 0f;
            thinkingText.gameObject.SetActive(false);
        }
        
        // Setup dialogue UI
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (continueText != null)
            continueText.text = "Press F to continue";
        
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
        }
        
        // Initially disable new models (they will appear after closet interaction)
        if (newMarcelo != null) newMarcelo.SetActive(false);
        if (newElio != null) newElio.SetActive(false);
        if (newValentina != null) newValentina.SetActive(false);
        if (newSamael != null) newSamael.SetActive(false);
        if (oldSamael != null) oldSamael.SetActive(true);
        
        // Make sure old models are enabled at start
        if (oldMarcelo != null) oldMarcelo.SetActive(true);
        if (oldElio != null) oldElio.SetActive(true);
        
        // Initially lock entrance door until chair interaction is done
        LockEntranceDoor();
        
        Debug.Log("=== DINING ROOM INTERACTION READY ===");
    }
    
    void LockEntranceDoor()
    {
        if (entranceDoor != null)
        {
            if (entranceDoorCollider != null) entranceDoorCollider.enabled = false;
            if (entranceDoorScript != null) entranceDoorScript.enabled = false;
            Debug.Log("Entrance(1) LOCKED until chair interaction");
        }
    }
    
    void UnlockEntranceDoor()
    {
        if (entranceDoor != null)
        {
            if (entranceDoorCollider != null) entranceDoorCollider.enabled = true;
            if (entranceDoorScript != null) entranceDoorScript.enabled = true;
            Debug.Log("Entrance(1) UNLOCKED - can now exit to outside");
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
        
        if (waitingForF && Input.GetKeyDown(KeyCode.F))
        {
            waitingForF = false;
        }
        
        // Only allow dining room interaction AFTER closet sequence is done and chair not used yet
        if (!hasInteracted && !isSequenceRunning && playerInTrigger && closetSequenceCompleted && !chairInteractionDone && Input.GetKeyDown(interactKey))
        {
            StartCoroutine(RunSequence());
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
                    thinkingText.text = hurryText;
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
    
    void RemoveDiningRoomModels()
    {
        Debug.Log("=== REMOVING DINING ROOM MODELS (except Samael) ===");
        if (newMarcelo != null) newMarcelo.SetActive(false);
        if (newElio != null) newElio.SetActive(false);
        if (newValentina != null) newValentina.SetActive(false);
        // Samael stays
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
    
    // Public method to show thinking text when pressing F on door
    public void ShowDoorThinkingText()
    {
        if (!chairInteractionDone)
        {
            StartCoroutine(ShowDoorThinkingTextCoroutine());
        }
    }
    
    // Public method to check if chair interaction is done (for entrance door)
    public bool IsChairInteractionDone()
    {
        return chairInteractionDone;
    }
    
    // Public method to start Samael dialogue when exiting through entrance door
    public void StartSamaelDialogue()
    {
        if (!samaelDialogueDone && chairInteractionDone)
        {
            StartCoroutine(SamaelDialogueSequence());
        }
    }
    
    IEnumerator ShowDoorThinkingTextCoroutine()
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
    
    IEnumerator ShowDialogue(string line, string speaker)
    {
        if (dialoguePanel == null)
        {
            Debug.LogError("DialoguePanel is NULL!");
            yield break;
        }
        
        dialoguePanel.SetActive(true);
        speakerText.text = speaker + ":";
        dialogueText.text = "";
        
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        
        if (continueText != null)
        {
            continueText.gameObject.SetActive(true);
        }
        
        waitingForF = true;
        yield return new WaitUntil(() => !waitingForF);
        
        if (continueText != null)
        {
            continueText.gameObject.SetActive(false);
        }
        
        dialoguePanel.SetActive(false);
    }
    
    IEnumerator SamaelDialogueSequence()
    {
        Debug.Log("=== SAMAEL DIALOGUE SEQUENCE STARTED ===");
        
        samaelDialogueDone = true;
        FreezePlayer();
        
        // Dialogue sequence
        yield return StartCoroutine(ShowDialogue(samaelDialogueLines[0], "Samael"));
        yield return StartCoroutine(ShowDialogue(metzlyDialogueLines[0], "Metzly"));
        yield return StartCoroutine(ShowDialogue(samaelDialogueLines[1], "Samael"));
        yield return StartCoroutine(ShowDialogue(metzlyDialogueLines[1], "Metzly"));
        yield return StartCoroutine(ShowDialogue(metzlyDialogueLines[2], "Metzly"));
        yield return StartCoroutine(ShowDialogue(samaelDialogueLines[2], "Samael"));
        yield return StartCoroutine(ShowDialogue(metzlyDialogueLines[2], "Metzly"));
        yield return StartCoroutine(ShowDialogue(samaelDialogueLines[3], "Samael"));
        yield return StartCoroutine(ShowDialogue(samaelDialogueLines[4], "Samael"));
        
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
            Debug.Log("Faded to black");
        }
        
        // Remove old Samael model
        if (oldSamael != null)
        {
            oldSamael.SetActive(false);
            Debug.Log("Old Samael model removed");
        }
        
        // Fade back
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
            Debug.Log("Faded back");
        }
        
        UnfreezePlayer();
        
        Debug.Log("=== SAMAEL DIALOGUE SEQUENCE COMPLETED ===");
    }
    
    IEnumerator RunSequence()
    {
        Debug.Log("=== DINING ROOM SEQUENCE STARTED ===");
        
        hasInteracted = true;
        isSequenceRunning = true;
        chairInteractionDone = true;
        
        // Hide prompt
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
            Debug.Log("Faded to black");
        }
        
        // Show story text
        if (thinkingText != null)
        {
            RectTransform rect = thinkingText.GetComponent<RectTransform>();
            if (rect != null)
            {
                Vector2 size = rect.sizeDelta;
                size.x = 1200f;
                rect.sizeDelta = size;
            }
            
            thinkingText.gameObject.SetActive(true);
            thinkingText.text = storyText;
            thinkingCanvasGroup.alpha = 1f;
            yield return new WaitForSeconds(8f);
            thinkingText.gameObject.SetActive(false);
        }
        
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
            Debug.Log("Faded back");
        }
        
        // Show final thinking text
        yield return StartCoroutine(ShowThinkingText(thinkingAfterText, thinkingAfterDuration));
        
        // Remove dining room models (except Samael)
        RemoveDiningRoomModels();
        
        // Unlock doors
        UnlockDoors();
        UnlockEntranceDoor();
        
        UnfreezePlayer();
        isSequenceRunning = false;
        
        // Disable the trigger so player can't interact again
        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }
        
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
