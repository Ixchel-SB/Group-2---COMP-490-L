using UnityEngine;

public class GraveInteraction : MonoBehaviour
{
    public GameObject interactionPrompt;
    private bool canDig = false;
    private bool hasDug = false;
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
    
    public void SetCanDig(bool can)
    {
        canDig = can;
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
        Debug.Log("Player dug at the grave - nothing else found");
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canDig && !hasDug)
        {
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
            if (interactionPrompt != null)
            {
                if (promptCanvasGroup != null)
                    promptCanvasGroup.alpha = 0f;
                interactionPrompt.SetActive(false);
            }
        }
    }
}
