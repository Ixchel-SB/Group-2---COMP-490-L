using UnityEngine;
using TMPro;
using System.Collections;

public class PostPhotoSequence : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI thinkingText;
    public GameObject blackScreenPanel;
    
    [Header("Arrow Interaction")]
    public GameObject arrowObject1;
    
    [Header("Door Lock")]
    private DormDoorInteraction exitDoor;
    
    [Header("Girls Room Door")]
    public MonoBehaviour girlsRoomDoor;
    
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
    private bool arrowPressed = false;  // ADD THIS
    
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
        else
        {
            Debug.LogError("ThinkingText is NULL in PostPhotoSequence!");
        }
        
        if (blackScreenPanel != null)
        {
            blackCanvasGroup = blackScreenPanel.GetComponent<CanvasGroup>();
            if (blackCanvasGroup == null)
                blackCanvasGroup = blackScreenPanel.AddComponent<CanvasGroup>();
            blackCanvasGroup.alpha = 0f;
        }
        else
        {
            Debug.LogError("BlackScreenPanel is NULL in PostPhotoSequence!");
        }
        
        if (arrowObject1 != null)
        {
            arrowObject1.SetActive(false);
        }
        else
        {
            Debug.LogError("ArrowObject1 is NOT assigned in PostPhotoSequence Inspector!");
        }
    }
    
    void Update()
    {
        if (waitingForF && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F pressed during dialogue - continuing");
            waitingForF = false;
        }
    }
    
    public void SetExitDoor(DormDoorInteraction door)
    {
        exitDoor = door;
        Debug.Log("Exit door reference set in PostPhotoSequence");
    }
    
    public void StartSequence()
    {
        if (hasStarted) return;
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
    
    public void ShowDoorBlockMessage()
    {
        StartCoroutine(DisplayDoorBlockMessage());
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
        Debug.Log("=== POST-PHOTO SEQUENCE STARTED ===");
        
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
            Debug.Log("Arrow enabled on closet");
        }
        
        // Wait for player to press F on arrow
        Debug.Log("Waiting for player to press F on arrow...");
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.F));
        arrowPressed = true;
        Debug.Log("F pressed on arrow - continuing sequence");
        
        if (arrowObject1 != null)
            arrowObject1.SetActive(false);
        
        // Time text 1 (Sunday 3:30pm)
        yield return StartCoroutine(ShowTimeText(timeText1));
        
        // Self dialogue lines
        for (int i = 0; i < selfDialogueLines.Length; i++)
        {
            yield return StartCoroutine(ShowDialogueLine(selfDialogueLines[i], "Meztli"));
        }
        
        // Valentina calls
        for (int i = 0; i < valentinaLines.Length; i++)
        {
            yield return StartCoroutine(ShowDialogueLine(valentinaLines[i], "Valentina"));
        }
        
        // Re-enable the girls room door after Valentina's calls
        if (girlsRoomDoor != null)
        {
            girlsRoomDoor.enabled = true;
            Debug.Log("Girls room door RE-ENABLED - player can now leave");
        }
        
        // Wait for player to press F on the door to teleport
        Debug.Log("Waiting for player to press F on the door...");
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.F));
        Debug.Log("F pressed on door - continuing sequence");
        
        // Story text
        yield return StartCoroutine(ShowStoryText());
        
        // Time text 2 (Monday 7:15am)
        yield return StartCoroutine(ShowTimeText(timeText2));
        
        // Unlock the exit door at the end
        if (exitDoor != null)
        {
            exitDoor.UnlockDoor();
            Debug.Log("Exit door UNLOCKED");
        }
        
        isSequenceRunning = false;
        Debug.Log("=== POST-PHOTO SEQUENCE COMPLETED ===");
    }
    
    IEnumerator ShowTimeText(string timeText)
    {
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
        
        // Show text
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = timeText;
        thinkingCanvasGroup.alpha = 1f;
        
        yield return new WaitForSeconds(3f);
        
        thinkingText.gameObject.SetActive(false);
        
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
        }
    }
    
    IEnumerator ShowDialogueLine(string line, string speaker)
    {
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = $"{speaker}: {line}";
        thinkingCanvasGroup.alpha = 1f;
        
        waitingForF = true;
        yield return new WaitUntil(() => !waitingForF);
        
        thinkingText.gameObject.SetActive(false);
    }
    
    IEnumerator ShowStoryText()
    {
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
        
        // Show story text
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = storyText;
        thinkingCanvasGroup.alpha = 1f;
        
        yield return new WaitForSeconds(5f);
        
        thinkingText.gameObject.SetActive(false);
        
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
        }
    }
}
