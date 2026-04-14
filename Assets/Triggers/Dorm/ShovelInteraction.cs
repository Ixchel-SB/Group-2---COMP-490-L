using UnityEngine;
using System.Collections;

public class ShovelInteraction : MonoBehaviour
{
    public GameObject interactionPrompt;
    public DormManager dormManager;
    public GameObject blackScreenPanel;
    public float fadeDuration = 0.5f;
    
    private bool playerInRange = false;
    private bool hasPickedUp = false;
    private CanvasGroup promptCanvasGroup;
    private CanvasGroup blackCanvasGroup;
    
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
        if (playerInRange && !hasPickedUp && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(PickUpShovel());
        }
    }
    
    IEnumerator PickUpShovel()
    {
        hasPickedUp = true;
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        
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
        
        yield return new WaitForSeconds(0.5f);
        
        // Notify DormManager that photo was found
        if (dormManager != null)
            dormManager.OnPhotoFound();
        
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
        
        // Hide shovel
        gameObject.SetActive(false);
        
        Debug.Log("Player picked up the shovel and found a photo!");
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPickedUp)
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
