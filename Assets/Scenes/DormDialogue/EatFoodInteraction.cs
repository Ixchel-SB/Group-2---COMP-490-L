using UnityEngine;

public class EatFoodInteraction : MonoBehaviour
{
    public GameObject interactionPrompt;
    public DormManager dormManager;
    
    private bool playerInRange = false;
    private bool hasEaten = false;
    private CanvasGroup promptCanvasGroup;
    
    void Start()
    {
        // Start disabled until Valentina is talked to
        enabled = false;
        
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
        if (playerInRange && !hasEaten && Input.GetKeyDown(KeyCode.F))
        {
            Eat();
        }
    }
    
    void Eat()
    {
        hasEaten = true;
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        
        if (dormManager != null)
            dormManager.EatFood();
        
        gameObject.SetActive(false);
        Debug.Log("Player ate the food");
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasEaten)
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
