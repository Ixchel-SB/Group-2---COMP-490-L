using UnityEngine;

public class GraveInteraction : MonoBehaviour
{
    public GameObject interactionPrompt;
    private bool canDig = false;
    private bool hasDug = false;
    private CanvasGroup promptCanvasGroup;
    
    void Start()
    {
        Debug.Log("GraveInteraction started - canDig: " + canDig);
        
        if (interactionPrompt != null)
        {
            promptCanvasGroup = interactionPrompt.GetComponent<CanvasGroup>();
            if (promptCanvasGroup == null)
                promptCanvasGroup = interactionPrompt.AddComponent<CanvasGroup>();
            promptCanvasGroup.alpha = 0f;
            interactionPrompt.SetActive(false);
        }
    }
    
    public void SetCanDig(bool can)
    {
        canDig = can;
        Debug.Log("Grave canDig set to: " + canDig);
        
        // If canDig is true and player is in range, show prompt
        if (canDig && interactionPrompt != null)
        {
            // Prompt will be shown on trigger enter
        }
    }
    
    void Update()
    {
        if (canDig && !hasDug && interactionPrompt != null && interactionPrompt.activeSelf && Input.GetKeyDown(KeyCode.F))
        {
            Dig();
        }
    }
    
    void Dig()
    {
        hasDug = true;
        Debug.Log("Player dug at the grave");
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canDig && !hasDug)
        {
            Debug.Log("Player in range of grave - can dig");
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
            Debug.Log("Player left range of grave");
            if (interactionPrompt != null)
            {
                if (promptCanvasGroup != null)
                    promptCanvasGroup.alpha = 0f;
                interactionPrompt.SetActive(false);
            }
        }
    }
}
