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
    
    void Start()
    {
        Debug.Log("PostPhotoSequence Start() called");
        
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
            Debug.Log("ArrowObject1 initialized - disabled");
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
        Debug.Log("StartDoorSequence called - arrowPressed: " + arrowPressed + ", doorSequenceTriggered: " + doorSequenceTriggered);
        
        if (!arrowPressed || doorSequenceTriggered) return;
        doorSequenceTriggered = true;
        StartCoroutine(RunDoorSequence());
    }
    
    IEnumerator DisplayDoorBlockMessage()
    {
        if (!arrowPressed)
        {
            thinkingText.gameObject.SetActive(true);
            thinkingText.text = doorBlockMessage;
            thinkingCanvasGroup.alpha = 1f;
            yield return new WaitForSeconds(2f);
            thinkingText.gameObject.SetActive(false);
        }
    }
    
    IEnumerator RunSequence()
    {
        Debug.Log("=== RUNSEQUENCE STARTED ===");
        
        // Lock the exit door
        if (exitDoor != null)
        {
            exitDoor.LockDoor();
            Debug.Log("Exit door LOCKED");
        }
        
        // Disable the girls room door so player can't leave
        if (girlsRoomDoor != null)
        {
            girlsRoomDoor.enabled = false;
            Debug.Log("Girls room door DISABLED");
        }
        
        // Show arrow on closet
        if (arrowObject1 != null)
        {
            arrowObject1.SetActive(true);
            Debug.Log("Arrow ENABLED on closet - Current state: " + arrowObject1.activeSelf);
        }
        else
        {
            Debug.LogError("ArrowObject1 is NULL - cannot enable arrow!");
        }
        
        // Wait for player to press F on arrow
        Debug.Log("Waiting for player to press F on arrow...");
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.F));
        arrowPressed = true;
        Debug.Log("F pressed on arrow - Arrow pressed = true");
        
        if (arrowObject1 != null)
            arrowObject1.SetActive(false);
        
        // Self dialogue lines
        for (int i = 0; i < selfDialogueLines.Length; i++)
        {
            yield return StartCoroutine(ShowDialogue(selfDialogueLines[i], "Meztly"));
        }
        
        // Valentina calls
        for (int i = 0; i < valentinaLines.Length; i++)
        {
            yield return StartCoroutine(ShowDialogue(valentinaLines[i], "Valentina"));
        }
        
        // Re-enable the girls room door after Valentina's calls
        if (girlsRoomDoor != null)
        {
            girlsRoomDoor.enabled = true;
            Debug.Log("Girls room door RE-ENABLED");
        }
        
        // Unlock exit door
        if (exitDoor != null)
        {
            exitDoor.UnlockDoor();
            Debug.Log("Exit door UNLOCKED");
        }
        
        isSequenceRunning = false;
        Debug.Log("=== POST-PHOTO SEQUENCE COMPLETED - Door sequence ready ===");
    }
    
    IEnumerator RunDoorSequence()
    {
        Debug.Log("=== DOOR SEQUENCE STARTED ===");
        
        // Disable the door during sequence
        if (girlsRoomDoor != null)
            girlsRoomDoor.enabled = false;
        
        // Time text 1
        yield return StartCoroutine(ShowTimeText(timeText1));
        
        // Story text
        yield return StartCoroutine(ShowStoryText());
        
        // Time text 2
        yield return StartCoroutine(ShowTimeText(timeText2));
        
        // Teleport player to bed
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && bedSpawnPoint != null)
        {
            player.transform.position = bedSpawnPoint.position;
            player.transform.rotation = bedSpawnPoint.rotation;
            Debug.Log("Player teleported to bed");
        }
        
        // Re-enable the door
        if (girlsRoomDoor != null)
            girlsRoomDoor.enabled = true;
        
        Debug.Log("=== DOOR SEQUENCE COMPLETED ===");
    }
    
    IEnumerator ShowDialogue(string line, string speaker)
    {
        if (dialoguePanel == null)
        {
            thinkingText.gameObject.SetActive(true);
            thinkingText.text = $"{speaker}: {line}";
            thinkingCanvasGroup.alpha = 1f;
            waitingForF = true;
            yield return new WaitUntil(() => !waitingForF);
            thinkingText.gameObject.SetActive(false);
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
        
        waitingForF = true;
        yield return new WaitUntil(() => !waitingForF);
        
        dialoguePanel.SetActive(false);
    }
    
    IEnumerator ShowTimeText(string timeText)
    {
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
        
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = timeText;
        thinkingCanvasGroup.alpha = 1f;
        
        yield return new WaitForSeconds(3f);
        
        thinkingText.gameObject.SetActive(false);
        
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
    }
    
    IEnumerator ShowStoryText()
    {
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
        
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = storyText;
        thinkingCanvasGroup.alpha = 1f;
        
        yield return new WaitForSeconds(5f);
        
        thinkingText.gameObject.SetActive(false);
        
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
    }
}
