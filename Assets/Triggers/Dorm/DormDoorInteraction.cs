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
    public float messageDuration = 3f;
    
    private bool playerInRange = false;
    private GameObject player;
    private CanvasGroup promptCanvasGroup;
    private bool isTransitioning = false;
    private CanvasGroup blackCanvasGroup;
    
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
        
        if (player != null && insideDormPosition != null)
        {
            player.transform.position = insideDormPosition.position;
            player.transform.rotation = insideDormPosition.rotation;
            Debug.Log("Player moved to inside dorm position");
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
        
        Debug.Log("Fade complete - player inside dorm");
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
