using UnityEngine;
using TMPro;
using System.Collections;

public class PostPhotoSequence : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI thinkingText;
    public GameObject blackScreenPanel;
    
    [Header("Dialogue UI Elements")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI continueText;
    
    [Header("Arrow Interaction")]
    public GameObject arrowObject1;
    
    [Header("Door Lock")]
    private DormDoorInteraction exitDoor;
    
    [Header("Girls Room Door")]
    public RoomDoorInteract girlsRoomDoor;  // Changed from RoomDoorInteraction to RoomDoorInteract
    
    [Header("Self Dialogue Lines")]
    public string[] selfDialogueLines = new string[]
    {
        "It's 3:30pm already. At least I finally finished unpacking my things while Valentina prepared dinner...",
        "I'm lucky Valentina was kind enough to explain to me our syllabus and schedule. I can't believe I have to read this whole book by the end of next week... I have so much to do",
        "And I haven't been able to stop thinking about the incident ever since I got here. I was barely able to get off the train knowing the accident wasn't that far away from the train station..."
    };
    
    [Header("Valentina Lines")]
    public string[] valentinaLines = new string[]
    {
        "DINNER IS READY!",
        "BETTER HURRY UP BEFORE IT'S GONE!"
    };
    
    private CanvasGroup thinkingCanvasGroup;
    private CanvasGroup blackCanvasGroup;
    private bool isSequenceRunning = false;
    private bool waitingForF = false;
    private bool hasStarted = false;
    private bool arrowPressed = false;
    private bool doorSequenceTriggered = false;
    
    // Player freeze references
    private GameObject player;
    private MonoBehaviour playerController;
    
    void Start()
    {
        Debug.Log("PostPhotoSequence Start() called");
        
        // Find player for freezing
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<MonoBehaviour>();
            if (playerController == null)
            {
                Transform playerArmature = player.transform.Find("PlayerArmature");
                if (playerArmature != null)
                    playerController = playerArmature.GetComponent<MonoBehaviour>();
            }
        }
        
        if (thinkingText != null)
        {
            thinkingCanvasGroup = thinkingText.GetComponent<CanvasGroup>();
            if (thinkingCanvasGroup == null)
                thinkingCanvasGroup = thinkingText.gameObject.AddComponent<CanvasGroup>();
            thinkingCanvasGroup.alpha = 0f;
            thinkingText.gameObject.SetActive(false);
        }
        
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (continueText != null)
            continueText.text = "Press F to continue";
        
        if (blackScreenPanel != null)
        {
            blackCanvasGroup = blackScreenPanel.GetComponent<CanvasGroup>();
            if (blackCanvasGroup == null)
                blackCanvasGroup = blackScreenPanel.AddComponent<CanvasGroup>();
            blackCanvasGroup.alpha = 0f;
        }
        
        if (arrowObject1 != null)
        {
            arrowObject1.SetActive(false);
        }
        else
        {
            Debug.LogError("ArrowObject1 is NULL! Please assign it in the Inspector.");
        }
    }
    
    void Update()
    {
        if (waitingForF && Input.GetKeyDown(KeyCode.F))
        {
            waitingForF = false;
        }
    }
    
    public bool IsSequenceRunning()
    {
        return isSequenceRunning;
    }
    
    public bool HasArrowPressed()
    {
        return arrowPressed;
    }
    
    public bool CanUseDoorAfterSequence()
    {
        return arrowPressed && !doorSequenceTriggered;
    }
    
    public void StartDoorSequence()
    {
        if (!arrowPressed || doorSequenceTriggered) return;
        doorSequenceTriggered = true;
        Debug.Log("Door sequence started - sequence complete, door can be used normally now");
    }
    
    public void ShowDoorBlockMessage()
    {
        if (!arrowPressed)
        {
            StartCoroutine(DisplayDoorBlockMessage());
        }
    }
    
    IEnumerator DisplayDoorBlockMessage()
    {
        if (thinkingText != null)
        {
            thinkingText.gameObject.SetActive(true);
            thinkingText.text = "I should finish unpacking...";
            thinkingCanvasGroup.alpha = 1f;
            yield return new WaitForSeconds(2f);
            thinkingText.gameObject.SetActive(false);
        }
    }
    
    public void SetExitDoor(DormDoorInteraction door)
    {
        exitDoor = door;
    }
    
    public void StartSequence()
    {
        Debug.Log("=== StartSequence() CALLED ===");
        
        if (hasStarted) 
        {
            Debug.Log("Sequence already started - ignoring");
            return;
        }
        
        hasStarted = true;
        isSequenceRunning = true;
        StartCoroutine(RunSequence());
    }
    
    public void OnArrowPressed()
    {
        if (arrowPressed) return;
        arrowPressed = true;
        Debug.Log("Arrow pressed - starting dialogue");
        StartCoroutine(PlayArrowDialogue());
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
    
    IEnumerator PlayArrowDialogue()
    {
        Debug.Log("=== PLAYING ARROW DIALOGUE ===");
        
        FreezePlayer();
        
        if (arrowObject1 != null)
        {
            arrowObject1.SetActive(false);
        }
        
        for (int i = 0; i < selfDialogueLines.Length; i++)
        {
            yield return StartCoroutine(ShowDialogue(selfDialogueLines[i], "Meztly"));
        }
        
        for (int i = 0; i < valentinaLines.Length; i++)
        {
            yield return StartCoroutine(ShowDialogue(valentinaLines[i], "Valentina"));
        }
        
        UnfreezePlayer();
        
        if (girlsRoomDoor != null)
        {
            girlsRoomDoor.enabled = true;
        }
        
        if (exitDoor != null)
        {
            exitDoor.UnlockDoor();
        }
        
        isSequenceRunning = false;
        Debug.Log("=== POST-PHOTO SEQUENCE COMPLETED ===");
    }
    
    IEnumerator ShowDialogue(string line, string speaker)
    {
        Debug.Log($"Showing dialogue: {speaker}: {line}");
        
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
    
    IEnumerator RunSequence()
    {
        Debug.Log("=== RUNSEQUENCE STARTED ===");
        
        if (exitDoor != null)
        {
            exitDoor.LockDoor();
        }
        
        if (girlsRoomDoor != null)
        {
            girlsRoomDoor.enabled = false;
        }
        
        if (arrowObject1 != null)
        {
            arrowObject1.SetActive(true);
            Transform prism = arrowObject1.transform.Find("Prism");
            if (prism != null)
            {
                prism.gameObject.SetActive(true);
                Renderer prismRenderer = prism.GetComponent<Renderer>();
                if (prismRenderer != null) prismRenderer.enabled = true;
            }
        }
        
        Debug.Log("=== WAITING FOR ARROW PRESS ===");
        
        while (!arrowPressed)
        {
            yield return null;
        }
        
        Debug.Log("Arrow pressed - continuing");
    }
}
