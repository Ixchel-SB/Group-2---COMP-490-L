using UnityEngine;
using System.Collections;

public class RoomDoorInteract : MonoBehaviour
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
            PostPhotoSequence postSequence = FindObjectOfType<PostPhotoSequence>();
            
            if (postSequence != null && postSequence.HasArrowPressed() && postSequence.CanUseDoorAfterSequence())
            {
                postSequence.StartDoorSequence();
                return;
            }
            
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
        
        // Find DiningRoomInteraction
        DiningRoomInteraction diningRoom = FindObjectOfType<DiningRoomInteraction>();
        
        // Check if this is the entrance door - looking at GameObject name
        bool isEntranceDoor = (gameObject.name == "Entrance(1)" || gameObject.name.Contains("Entrance"));
        
        Debug.Log($"Door name: {gameObject.name}, isEntranceDoor: {isEntranceDoor}");
        
        if (diningRoom != null)
        {
            Debug.Log($"Found DiningRoomInteraction, ChairInteractionDone: {diningRoom.IsChairInteractionDone()}");
        }
        else
        {
            Debug.LogWarning("DiningRoomInteraction not found in scene!");
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
        
        yield return new WaitForSeconds(waitTime);
        
        if (player != null && teleportPosition != null)
        {
            player.transform.position = teleportPosition.position;
            player.transform.rotation = teleportPosition.rotation;
            Debug.Log("Player teleported to: " + teleportPosition.name);
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
        
        // Start Samael dialogue AFTER teleport and fade back
        if (isEntranceDoor && diningRoom != null && diningRoom.IsChairInteractionDone())
        {
            Debug.Log(">>> STARTING SAMAEL DIALOGUE <<<");
            diningRoom.StartSamaelDialogue();
        }
        else
        {
            Debug.Log($"Samael dialogue NOT starting. isEntranceDoor={isEntranceDoor}, diningRoom={diningRoom != null}, chairDone={(diningRoom != null ? diningRoom.IsChairInteractionDone().ToString() : "N/A")}");
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
