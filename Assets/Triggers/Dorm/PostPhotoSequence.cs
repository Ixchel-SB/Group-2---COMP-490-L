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
    public RoomDoorInteraction girlsRoomDoor;
    public Transform bedSpawnPoint;
    
    [Header("Door Block Message")]
    public string doorBlockMessage = "I should finish unpacking...";
    
    [Header("Self Dialogue Lines")]
    public string[] selfDialogueLines = new string[]
    {
        "I finally finished unpacking my things while Valentina prepared dinner...",
        "I'm lucky Valentina was kind enough to explain to me our syllabus and schedule. I can't believe I have to read this whole book by the end of next week... I have so much to do",
        "And I haven't been able to stop thinking about the incident ever since I got here. I was barely able to get off the train knowing the accident wasn't that far away from the train station..."
    };
    
    [Header("Valentina Lines")]
    public string[] valentinaLines = new string[]
    {
        "DINNER IS READY!",
        "BETTER HURRY UP BEFORE IT'S GONE!"
    };
    
    [Header("Time Texts")]
    public string timeText1 = "Sunday 3:30pm";
    public string timeText2 = "Monday 7:15am";
    public string storyText = "Metzly and her roommates ate their dinner and began talking about starting school tomorrow.\n\nOnce they all finished, Metzly and Valentina took a bath and got all their supplies ready for school while the boys procrastinated the whole day.\n\nThe girls were able to go to bed early.";
    
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
    
    public void ShowDoorBlockMessage()
    {
        if (!arrowPressed)
        {
            StartCoroutine(DisplayDoorBlockMessage());
        }
    }
    
    public void StartDoorSequence()
    {
        if (!arrowPressed || doorSequenceTriggered) return;
        doorSequenceTriggered = true;
        StartCoroutine(RunDoorSequence());
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
        
        // Freeze player
        FreezePlayer();
        
        // Hide arrow
        if (arrowObject1 != null)
        {
            arrowObject1.SetActive(false);
        }
        
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
            Debug.Log("Screen black - dialogue will appear");
        }
        
        // Show dialogue lines on black screen
        for (int i = 0; i < selfDialogueLines.Length; i++)
        {
            yield return StartCoroutine(ShowDialogueOnBlackScreen(selfDialogueLines[i], "Meztly"));
        }
        
        // Show Valentina lines on black screen
        for (int i = 0; i < valentinaLines.Length; i++)
        {
            yield return StartCoroutine(ShowDialogueOnBlackScreen(valentinaLines[i], "Valentina"));
        }
        
        // Fade back to normal
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
            Debug.Log("Screen back to normal");
        }
        
        // Unfreeze player
        UnfreezePlayer();
        
        // Re-enable the girls room door
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
    
    IEnumerator ShowDialogueOnBlackScreen(string line, string speaker)
    {
        // Show dialogue panel
        dialoguePanel.SetActive(true);
        speakerText.text = speaker + ":";
        dialogueText.text = "";
        
        // Type out the text
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        
        // Show "Press F to continue" prompt
        if (continueText != null)
            continueText.gameObject.SetActive(true);
        
        // Wait for player to press F
        waitingForF = true;
        yield return new WaitUntil(() => !waitingForF);
        
        // Hide continue text
        if (continueText != null)
            continueText.gameObject.SetActive(false);
        
        // Hide dialogue panel
        dialoguePanel.SetActive(false);
    }
    
    IEnumerator DisplayDoorBlockMessage()
    {
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = doorBlockMessage;
        thinkingCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(2f);
        thinkingText.gameObject.SetActive(false);
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
        
        // Enable arrow
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
    
    IEnumerator RunDoorSequence()
    {
        Debug.Log("=== DOOR SEQUENCE STARTED ===");
        
        // Freeze player
        FreezePlayer();
        
        if (girlsRoomDoor != null)
            girlsRoomDoor.enabled = false;
        
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
        
        // Time text 1
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = timeText1;
        thinkingCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(3f);
        thinkingText.gameObject.SetActive(false);
        
        // Story text
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = storyText;
        thinkingCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(5f);
        thinkingText.gameObject.SetActive(false);
        
        // Time text 2
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = timeText2;
        thinkingCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(3f);
        thinkingText.gameObject.SetActive(false);
        
        // Fade back to normal
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
        
        // Teleport player to bed
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null && bedSpawnPoint != null)
        {
            Transform root = playerObj.transform.root;
            root.position = bedSpawnPoint.position;
            root.rotation = bedSpawnPoint.rotation;
            playerObj.transform.position = bedSpawnPoint.position;
            
            CharacterController cc = playerObj.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                playerObj.transform.position = bedSpawnPoint.position;
                cc.enabled = true;
            }
        }
        
        // Unfreeze player
        UnfreezePlayer();
        
        if (girlsRoomDoor != null)
            girlsRoomDoor.enabled = true;
        
        Debug.Log("=== DOOR SEQUENCE COMPLETED ===");
    }
}
