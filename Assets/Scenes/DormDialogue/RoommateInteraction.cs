using UnityEngine;
using System.Collections;

public class RoommateInteraction : MonoBehaviour
{
    public RoommateDialogue roommateDialogue;
    public GameObject interactionPrompt;
    
    private bool playerInRange = false;
    private bool hasInteracted = false;
    private CanvasGroup promptCanvasGroup;
    
    void Start()
    {
        Debug.Log("RoommateInteraction started on: " + gameObject.name);
        
        if (interactionPrompt != null)
        {
            promptCanvasGroup = interactionPrompt.GetComponent<CanvasGroup>();
            if (promptCanvasGroup == null)
                promptCanvasGroup = interactionPrompt.AddComponent<CanvasGroup>();
            promptCanvasGroup.alpha = 0f;
            interactionPrompt.SetActive(false);
            Debug.Log("Interaction prompt initialized");
        }
        else
        {
            Debug.LogWarning("Interaction Prompt is not assigned on " + gameObject.name);
        }
        
        if (roommateDialogue == null)
        {
            Debug.LogWarning("Roommate Dialogue is not assigned on " + gameObject.name);
        }
    }
    
    void Update()
    {
        if (playerInRange && !hasInteracted && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F key pressed - interacting with " + gameObject.name);
            Interact();
        }
    }
    
    void Interact()
    {
        hasInteracted = true;
        Debug.Log("Interacting with " + gameObject.name);
        
        if (roommateDialogue != null)
        {
            roommateDialogue.StartDialogue();
            Debug.Log("Dialogue started");
        }
        else
        {
            Debug.LogError("RoommateDialogue is null on " + gameObject.name);
        }
        
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.name + " with tag: " + other.tag);
        
        if (other.CompareTag("Player") && !hasInteracted)
        {
            playerInRange = true;
            Debug.Log("Player in range - should show prompt");
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
            Debug.Log("Player left range");
            if (interactionPrompt != null)
            {
                StartCoroutine(FadeOutPrompt());
            }
        }
    }
    
    IEnumerator FadeInPrompt()
    {
        if (promptCanvasGroup == null) yield break;
        
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            promptCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.3f);
            yield return null;
        }
        promptCanvasGroup.alpha = 1f;
        Debug.Log("Prompt faded in");
    }
    
    IEnumerator FadeOutPrompt()
    {
        if (promptCanvasGroup == null) yield break;
        
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            promptCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.3f);
            yield return null;
        }
        promptCanvasGroup.alpha = 0f;
        interactionPrompt.SetActive(false);
        Debug.Log("Prompt faded out");
    }
}
