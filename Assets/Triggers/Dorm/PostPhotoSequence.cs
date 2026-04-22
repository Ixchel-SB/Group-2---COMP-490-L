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
    
    [Header("Dialogue Lines")]
    public string[] selfDialogueLines = new string[]
    {
        "I finally finished unpacking my things while Valentina prepared dinner...",
        "I'm lucky Valentina was kind enough to explain to me our syllabus and schedule. I can't believe I have to read this whole book by the end of next week... I have so much to do",
        "And I haven't been able to stop thinking about the incident ever since I got here. I was barely able to get off the train knowing the accident wasn't that far away from the train station..."
    };
    
    public string[] valentinaLines = new string[]
    {
        "DINNER IS READY!",
        "BETTER HURRY UP BEFORE IT'S GONE!"
    };
    
    public string timeText1 = "Sunday 3:30pm";
    public string timeText2 = "Monday 7:15am";
    public string storyText = "Metzly and her roommates ate their dinner and began talking about starting school tomorrow.\n\nOnce they all finished, Metzly and Valentina took a bath and got all their supplies ready for school while the boys procrastinated the whole day.\n\nThe girls were able to go to bed early.";
    
    private CanvasGroup thinkingCanvasGroup;
    private CanvasGroup blackCanvasGroup;
    private bool isSequenceRunning = false;
    private bool waitingForF = false;
    private bool hasStarted = false;
    
    void Start()
    {
        if (thinkingText != null)
        {
            thinkingCanvasGroup = thinkingText.GetComponent<CanvasGroup>();
            if (thinkingCanvasGroup == null)
                thinkingCanvasGroup = thinkingText.gameObject.AddComponent<CanvasGroup>();
            thinkingCanvasGroup.alpha = 0f;
            thinkingText.gameObject.SetActive(false);
        }
        
        if (blackScreenPanel != null)
        {
            blackCanvasGroup = blackScreenPanel.GetComponent<CanvasGroup>();
            if (blackCanvasGroup == null)
                blackCanvasGroup = blackScreenPanel.AddComponent<CanvasGroup>();
            blackCanvasGroup.alpha = 0f;
        }
        
        if (arrowObject1 != null)
            arrowObject1.SetActive(false);
    }
    
    void Update()
    {
        if (waitingForF && Input.GetKeyDown(KeyCode.F))
        {
            waitingForF = false;
            ContinueSequence();
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
    
    IEnumerator RunSequence()
    {
        Debug.Log("Post-photo sequence started");
        
        // Lock the exit door at the beginning of the sequence
        if (exitDoor != null)
        {
            exitDoor.LockDoor();
            Debug.Log("Exit door LOCKED during post-photo sequence");
        }
        
        // Show arrow on closet
        if (arrowObject1 != null)
        {
            arrowObject1.SetActive(true);
            Debug.Log("Arrow enabled on closet - press F to continue");
        }
        
        // Wait for player to press F on arrow
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.F));
        
        if (arrowObject1 != null)
            arrowObject1.SetActive(false);
        
        // Time text 1
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
        
        // Story text
        yield return StartCoroutine(ShowStoryText());
        
        // Time text 2
        yield return StartCoroutine(ShowTimeText(timeText2));
        
        // Unlock the exit door at the end of the sequence
        if (exitDoor != null)
        {
            exitDoor.UnlockDoor();
            Debug.Log("Exit door UNLOCKED - sequence complete");
        }
        
        isSequenceRunning = false;
        Debug.Log("Post-photo sequence completed");
    }
    
    void ContinueSequence()
    {
        // Continue is handled by the coroutine waiting
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
