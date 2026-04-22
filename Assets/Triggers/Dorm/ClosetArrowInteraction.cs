using UnityEngine;

public class ClosetArrowInteraction : MonoBehaviour
{
    public PostPhotoSequence postPhotoSequence;
    public GameObject interactionPrompt;
    
    private bool playerInRange = false;
    private CanvasGroup promptCanvasGroup;
    private bool hasInteracted = false;
    
    void Start()
    {
        if (interactionPrompt != null)
        {
            promptCanvasGroup = interactionPrompt.GetComponent<CanvasGroup>();
            if (promptCanvasGroup == null)
                promptCanvasGroup = interactionPrompt.AddComponent<CanvasGroup>();
            promptCanvasGroup.alpha = 0f;
            interactionPrompt.SetActive(false);
        }
        
        // Start disabled - will be enabled after photo inspection
        gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (playerInRange && !hasInteracted && Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }
    
    void Interact()
    {
        hasInteracted = true;
        Debug.Log("Closet arrow interacted - starting post-photo sequence");
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        
        if (postPhotoSequence != null)
        {
            postPhotoSequence.StartSequence();
        }
        
        // Hide the arrow
        gameObject.SetActive(false);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasInteracted)
        {
            playerInRange = true;
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
            if (interactionPrompt != null)
            {
                if (promptCanvasGroup != null)
                    promptCanvasGroup.alpha = 0f;
                interactionPrompt.SetActive(false);
            }
        }
    }
}
