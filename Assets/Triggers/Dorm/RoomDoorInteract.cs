using UnityEngine;
using System.Collections;

public class RoomDoorInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    public string doorName = "Door";
    public string interactionMessage = "Press F to open door";
    public GameObject interactionPrompt;
    public Transform teleportPosition;
    public GameObject blackScreenPanel;
    public float fadeDuration = 0.5f;
    public float waitTime = 1f;
    
    [Header("Requirements")]
    public bool requireValentinaTalked = false;
    public string blockMessage = "I should talk with Valentina first before going inside";
    
    [Header("Block Message UI")]
    public DoorBlockMessage doorBlockMessage;
    
    private bool playerInRange = false;
    private GameObject player;
    private CanvasGroup promptCanvasGroup;
    private bool isTransitioning = false;
    private CanvasGroup blackCanvasGroup;
    private float lastInteractionTime = 0f;
    private float interactionCooldown = 1f;
    
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
    }
    
    void Update()
    {
        if (playerInRange && !isTransitioning && Input.GetKeyDown(KeyCode.F))
        {
            // Check if post-photo sequence is active and arrow not pressed yet
            PostPhotoSequence postSequence = FindObjectOfType<PostPhotoSequence>();
            if (postSequence != null && postSequence.IsSequenceRunning() && !postSequence.HasArrowPressed())
            {
                postSequence.ShowDoorBlockMessage();
                return;
            }
            
            if (Time.time - lastInteractionTime < interactionCooldown) return;
            lastInteractionTime = Time.time;
            
            if (CanUseDoor())
            {
                StartCoroutine(UseDoor());
            }
            else
            {
                ShowBlockMessage();
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
        return true;
    }
    
    void ShowBlockMessage()
    {
        DormManager dormManager = FindObjectOfType<DormManager>();
        if (dormManager != null && dormManager.IsValentinaTalked() && !dormManager.HasFoodEaten())
        {
            dormManager.ShowDoorEatReminder();
        }
        else if (doorBlockMessage != null)
        {
            doorBlockMessage.ShowMessage(blockMessage);
        }
        
        if (interactionPrompt != null)
        {
            StartCoroutine(TempHidePrompt());
        }
    }
    
    IEnumerator TempHidePrompt()
    {
        interactionPrompt.SetActive(false);
        yield return new WaitForSeconds(2f);
        if (playerInRange && !isTransitioning)
        {
            interactionPrompt.SetActive(true);
            if (promptCanvasGroup != null)
                promptCanvasGroup.alpha = 1f;
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
        }
        
        yield return new WaitForSeconds(waitTime);
        
        // Teleport player
        if (player != null && teleportPosition != null)
        {
            player.transform.position = teleportPosition.position;
            player.transform.rotation = teleportPosition.rotation;
            Debug.Log("Player teleported to: " + teleportPosition.name);
        }
        
        // Fade back in
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
                if (promptCanvasGroup != null)
                    promptCanvasGroup.alpha = 1f;
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
                if (promptCanvasGroup != null)
                    promptCanvasGroup.alpha = 0f;
                interactionPrompt.SetActive(false);
            }
        }
    }
}
