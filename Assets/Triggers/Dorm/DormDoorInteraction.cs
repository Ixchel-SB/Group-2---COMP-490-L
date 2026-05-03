using UnityEngine;
using TMPro;
using System.Collections;

public class DormDoorInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    public string interactionMessage = "Press F to enter dorm";
    public GameObject interactionPrompt;
    public Transform insideDormPosition;
    public GameObject blackScreenPanel;
    public float fadeDuration = 0.5f;
    public float waitTime = 3f;
    
    [Header("Requirements")]
    public bool requireVendorDialogue = true;
    public bool requireAllRoommatesTalked = false;
    
    [Header("Block Message")]
    public TextMeshProUGUI blockMessageText;
    public string blockMessage = "I don't want to go inside yet. I want to get something to eat first...";
    public string sequenceLockMessage = "I can't leave yet... something is happening.";
    public string cemeteryMessage = "I need to hurry to the cemetery before it gets late.";
    public float messageDuration = 3f;
    
    [Header("Samael Dialogue")]
    public bool isSamaelExitDoor = false;
    public Transform outsideDormPosition;
    
    private bool playerInRange = false;
    private GameObject player;
    private CanvasGroup promptCanvasGroup;
    private bool isTransitioning = false;
    private CanvasGroup blackCanvasGroup;
    private bool isLocked = false;
    private bool samaelDialogueTriggered = false;
    private bool samaelDialogueCompleted = false;
    
    void Start()
    {
        Time.timeScale = 1f;
        
        if (interactionPrompt != null)
        {
            promptCanvasGroup = interactionPrompt.GetComponent<CanvasGroup>();
            if (promptCanvasGroup == null)
                promptCanvasGroup = interactionPrompt.AddComponent<CanvasGroup>();
            promptCanvasGroup.alpha = 0f;
            interactionPrompt.SetActive(false);
        }
        
        if (blackScreenPanel != null)
        {
            blackCanvasGroup = blackScreenPanel.GetComponent<CanvasGroup>();
            if (blackCanvasGroup == null)
                blackCanvasGroup = blackScreenPanel.AddComponent<CanvasGroup>();
            blackCanvasGroup.alpha = 0f;
        }
        
        if (blockMessageText != null)
        {
            CanvasGroup cg = blockMessageText.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = blockMessageText.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            blockMessageText.gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        if (playerInRange && !isTransitioning && Input.GetKeyDown(KeyCode.F))
        {
            // If Samael dialogue is completed, show cemetery message
            if (samaelDialogueCompleted)
            {
                StartCoroutine(ShowCemeteryMessage());
                return;
            }
            
            if (isLocked)
            {
                StartCoroutine(ShowSequenceLockMessage());
                return;
            }
            
            if (CanUseDoor())
            {
                StartCoroutine(EnterDorm());
            }
            else
            {
                StartCoroutine(ShowBlockMessage());
            }
        }
    }
    
    IEnumerator ShowCemeteryMessage()
    {
        Debug.Log("Showing cemetery message - door locked permanently");
        
        if (blockMessageText != null)
        {
            CanvasGroup cg = blockMessageText.GetComponent<CanvasGroup>();
            blockMessageText.text = cemeteryMessage;
            blockMessageText.gameObject.SetActive(true);
            
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
                yield return null;
            }
            cg.alpha = 1f;
            
            yield return new WaitForSeconds(messageDuration);
            
            elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
                yield return null;
            }
            cg.alpha = 0f;
            blockMessageText.gameObject.SetActive(false);
        }
    }
    
    public void LockDoor()
    {
        isLocked = true;
        Debug.Log("Dorm door locked - cannot exit during sequence");
    }
    
    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("Dorm door unlocked - can exit now");
    }
    
    public void SetSamaelDialogueCompleted()
    {
        samaelDialogueCompleted = true;
        isLocked = true; // Keep door locked permanently
        Debug.Log("Samael dialogue completed - door permanently locked with cemetery message");
    }
    
    IEnumerator ShowSequenceLockMessage()
    {
        Debug.Log("Cannot exit - door is locked by sequence");
        
        if (blockMessageText != null)
        {
            CanvasGroup cg = blockMessageText.GetComponent<CanvasGroup>();
            blockMessageText.text = sequenceLockMessage;
            blockMessageText.gameObject.SetActive(true);
            
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
                yield return null;
            }
            cg.alpha = 1f;
            
            yield return new WaitForSeconds(messageDuration);
            
            elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
                yield return null;
            }
            cg.alpha = 0f;
            blockMessageText.gameObject.SetActive(false);
        }
    }
    
    bool CanUseDoor()
    {
        if (requireVendorDialogue)
        {
            if (GameProgressManager.Instance == null || !GameProgressManager.Instance.IsVendorDialogueCompleted())
            {
                return false;
            }
        }
        
        if (requireAllRoommatesTalked)
        {
            DormManager dormManager = FindObjectOfType<DormManager>();
            if (dormManager == null || !dormManager.AreAllRoommatesTalked())
            {
                return false;
            }
        }
        
        return true;
    }
    
    IEnumerator ShowBlockMessage()
    {
        Debug.Log("Cannot enter dorm - requirements not met");
        
        if (blockMessageText != null)
        {
            CanvasGroup cg = blockMessageText.GetComponent<CanvasGroup>();
            blockMessageText.text = blockMessage;
            blockMessageText.gameObject.SetActive(true);
            
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
                yield return null;
            }
            cg.alpha = 1f;
            
            yield return new WaitForSeconds(messageDuration);
            
            elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
                yield return null;
            }
            cg.alpha = 0f;
            blockMessageText.gameObject.SetActive(false);
        }
    }
    
    IEnumerator EnterDorm()
    {
        isTransitioning = true;
        Debug.Log("Entering dorm...");
        
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        
        DiningRoomInteraction diningRoom = FindObjectOfType<DiningRoomInteraction>();
        
        Debug.Log($"isSamaelExitDoor: {isSamaelExitDoor}");
        if (diningRoom != null)
        {
            Debug.Log($"ChairInteractionDone: {diningRoom.IsChairInteractionDone()}");
        }
        
        Transform targetPosition = insideDormPosition;
        bool shouldTriggerSamael = false;
        
        if (isSamaelExitDoor && diningRoom != null && diningRoom.IsChairInteractionDone() && !samaelDialogueTriggered)
        {
            targetPosition = outsideDormPosition;
            shouldTriggerSamael = true;
            Debug.Log("Using outside dorm position for Samael exit");
        }
        
        if (blackCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                blackCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }
            blackCanvasGroup.alpha = 1f;
        }
        
        Debug.Log("Screen black - waiting " + waitTime + " seconds");
        
        yield return new WaitForSeconds(waitTime);
        
        if (player != null && targetPosition != null)
        {
            player.transform.position = targetPosition.position;
            player.transform.rotation = targetPosition.rotation;
            Debug.Log("Player moved to: " + targetPosition.name);
        }
        
        if (blackCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                blackCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }
            blackCanvasGroup.alpha = 0f;
        }
        
        Debug.Log("Fade complete - player teleported");
        
        // Start Samael dialogue if this is the exit door and chair is done
        if (shouldTriggerSamael && diningRoom != null)
        {
            samaelDialogueTriggered = true;
            Debug.Log(">>> STARTING SAMAEL DIALOGUE <<<");
            
            // Lock the door during dialogue
            LockDoor();
            
            // Start the dialogue
            diningRoom.StartSamaelDialogue();
            
            // Wait for dialogue to complete (check every frame)
            yield return new WaitUntil(() => diningRoom.IsSamaelDialogueDone());
            
            Debug.Log("Samael dialogue completed - fading to black");
            
            // Fade to black
            if (blackCanvasGroup != null)
            {
                float elapsed = 0f;
                while (elapsed < fadeDuration)
                {
                    elapsed += Time.deltaTime;
                    blackCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                    yield return null;
                }
                blackCanvasGroup.alpha = 1f;
                
                yield return new WaitForSeconds(0.5f);
                
                elapsed = 0f;
                while (elapsed < fadeDuration)
                {
                    elapsed += Time.deltaTime;
                    blackCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                    yield return null;
                }
                blackCanvasGroup.alpha = 0f;
            }
            
            // Set Samael dialogue as completed - door stays locked permanently
            SetSamaelDialogueCompleted();
            Debug.Log("Samael dialogue completed - door permanently locked with cemetery message");
        }
        
        isTransitioning = false;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            playerInRange = true;
            player = other.gameObject;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
                StartCoroutine(FadeInPrompt());
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
            if (interactionPrompt != null)
            {
                StartCoroutine(FadeOutPrompt());
            }
        }
    }
    
    IEnumerator FadeInPrompt()
    {
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            promptCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.3f);
            yield return null;
        }
        promptCanvasGroup.alpha = 1f;
    }
    
    IEnumerator FadeOutPrompt()
    {
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            promptCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.3f);
            yield return null;
        }
        promptCanvasGroup.alpha = 0f;
        interactionPrompt.SetActive(false);
    }
}
