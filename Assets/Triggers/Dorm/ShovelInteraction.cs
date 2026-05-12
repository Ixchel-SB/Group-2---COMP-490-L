using UnityEngine;
using System.Collections;

public class ShovelInteraction : MonoBehaviour
{
    public GameObject interactionPrompt;
    public DormManager dormManager;
    public GameObject blackScreenPanel;
    public float fadeDuration = 0.5f;
    public float blackScreenHoldTime = 10f;
    public string pickupMessage = "It looks like something else was buried here";
    public DoorBlockMessage blockMessage;
    
    private bool playerInRange = false;
    private bool hasPickedUp = false;
    private CanvasGroup promptCanvasGroup;
    private CanvasGroup blackCanvasGroup;
    
    void Start()
    {
        gameObject.SetActive(true);
        Debug.Log("ShovelInteraction started");
        
        if (interactionPrompt != null)
        {
            promptCanvasGroup = interactionPrompt.GetComponent<CanvasGroup>();
            if (promptCanvasGroup == null)
                promptCanvasGroup = interactionPrompt.AddComponent<CanvasGroup>();
            promptCanvasGroup.alpha = 0f;
            interactionPrompt.SetActive(false);
        }
        else
        {
            Debug.LogWarning("InteractionPrompt not assigned on Shovel!");
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
            Debug.LogWarning("BlackScreenPanel not assigned on Shovel!");
        }
        
        if (dormManager == null)
        {
            Debug.LogError("DormManager not assigned on Shovel!");
        }
        
        if (blockMessage == null)
        {
            Debug.LogWarning("BlockMessage not assigned on Shovel!");
        }
    }
    
    void Update()
    {
        if (playerInRange && !hasPickedUp && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Picking up shovel!");
            StartCoroutine(PickUpShovel());
        }
    }
    
    IEnumerator PickUpShovel()
    {
        hasPickedUp = true;
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        
        //Show the pickup message
        if (blockMessage != null)
        {
            blockMessage.ShowMessage(pickupMessage);
            yield return new WaitForSeconds(2f);
        }
        
        //Fade to black
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
            Debug.Log("Faded to black");
        }
        
        //Hold black screen for 10 seconds (SHOULD CHANGE TIME IDK)
        yield return new WaitForSecondsRealtime(blackScreenHoldTime);
        
        //Fade back to normal!
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
            Debug.Log("Faded back to normal");
        }
        
        //Small delay to ensure screen is fully normal
        yield return new WaitForSecondsRealtime(0.5f);
        
        //Show photo for inspection
        if (dormManager != null)
        {
            Debug.Log("Calling dormManager.ShowPhotoForInspection()");
            dormManager.ShowPhotoForInspection();
        }
    }
    
    public void CompletePickup()
    {
        gameObject.SetActive(false);
        Debug.Log("Shovel has been hidden");
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPickedUp)
        {
            playerInRange = true;
            Debug.Log("Player in range of shovel");
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
            Debug.Log("Player left range of shovel");
            if (interactionPrompt != null)
            {
                if (promptCanvasGroup != null)
                    promptCanvasGroup.alpha = 0f;
                interactionPrompt.SetActive(false);
            }
        }
    }
}
