using UnityEngine;
using TMPro;
using System.Collections;

public class InteriorDoorInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    public string doorName = "Door";
    public string interactionMessage = "Press F to open door";
    public GameObject interactionPrompt;
    public Transform teleportPosition; // Where player goes when using door
    public GameObject blackScreenPanel;
    public float fadeDuration = 0.5f;
    public float waitTime = 1f;
    
    [Header("Requirements")]
    public bool requireValentinaTalked = false; // For Girl/Boy rooms
    public bool requireAllRoommatesTalked = false; // For Backyard
    public string blockMessage = "I should talk with Valentina first before going inside";
    
    private bool playerInRange = false;
    private GameObject player;
    private CanvasGroup promptCanvasGroup;
    private bool isTransitioning = false;
    private CanvasGroup blackCanvasGroup;
    private TextMeshProUGUI blockMessageText;
    private CanvasGroup blockMessageCanvasGroup;
    
    void Start()
    {
        // FORCE GAME TO RUN
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
        
        // Find the block message UI (reuse the one from DormDoorInteraction or create new)
        GameObject blockMsgObj = GameObject.Find("DoorBlockMessageText");
        if (blockMsgObj != null)
        {
            blockMessageText = blockMsgObj.GetComponent<TextMeshProUGUI>();
            blockMessageCanvasGroup = blockMsgObj.GetComponent<CanvasGroup>();
            if (blockMessageCanvasGroup == null)
                blockMessageCanvasGroup = blockMsgObj.AddComponent<CanvasGroup>();
            blockMessageCanvasGroup.alpha = 0f;
            blockMsgObj.SetActive(false);
        }
    }
    
    void Update()
    {
        if (playerInRange && !isTransitioning && Input.GetKeyDown(KeyCode.F))
        {
            if (CanUseDoor())
            {
                StartCoroutine(UseDoor());
            }
            else
            {
                StartCoroutine(ShowBlockMessage());
            }
        }
    }
    
    bool CanUseDoor()
    {
        if (requireValentinaTalked)
        {
            DormManager dormManager = FindObjectOfType<DormManager>();
            if (dormManager == null || !dormManager.IsValentinaTalked())
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
        Debug.Log("Cannot use " + doorName + " - " + blockMessage);
        
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        
        if (blockMessageText != null)
        {
            blockMessageText.text = blockMessage;
            blockMessageText.gameObject.SetActive(true);
            
            // Fade in
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                blockMessageCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
                yield return null;
            }
            blockMessageCanvasGroup.alpha = 1f;
            
            // Wait
            yield return new WaitForSeconds(3f);
            
            // Fade out
            elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                blockMessageCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
                yield return null;
            }
            blockMessageCanvasGroup.alpha = 0f;
            blockMessageText.gameObject.SetActive(false);
        }
        
        // Re-show prompt after message
        if (interactionPrompt != null && playerInRange)
        {
            yield return new WaitForSeconds(0.5f);
            interactionPrompt.SetActive(true);
        }
    }
    
    IEnumerator UseDoor()
    {
        isTransitioning = true;
        Debug.Log("Using door: " + doorName);
        
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        
        // FADE TO BLACK
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
        
        yield return new WaitForSeconds(waitTime);
        
        // TELEPORT PLAYER
        if (player != null && teleportPosition != null)
        {
            player.transform.position = teleportPosition.position;
            player.transform.rotation = teleportPosition.rotation;
            Debug.Log("Player teleported to: " + teleportPosition.name);
        }
        
        // FADE BACK IN
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
