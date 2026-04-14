using UnityEngine;
using System.Collections;

public class RoommateInteraction : MonoBehaviour
{
    public RoommateDialogue roommateDialogue;
    public GameObject interactionPrompt;
    
    private bool playerInRange = false;
    private bool hasInteracted = false;
    private bool hasNotified = false;
    private CanvasGroup promptCanvasGroup;
    
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
        
        if (roommateDialogue != null)
        {
            StartCoroutine(WaitForDialogueEnd());
            roommateDialogue.StartDialogue();
        }
        
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    IEnumerator WaitForDialogueEnd()
    {
        // Wait while dialogue is active
        while (roommateDialogue != null && roommateDialogue.IsDialogueActive())
        {
            yield return null;
        }
        
        // Now notify DormManager after dialogue ends
        if (roommateDialogue != null && roommateDialogue.roommateName == "Valentina" && !hasNotified)
        {
            hasNotified = true;
            DormManager dormManager = FindObjectOfType<DormManager>();
            if (dormManager != null)
            {
                dormManager.ValentinaFirstDialogueComplete();
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasInteracted)
        {
            playerInRange = true;
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
