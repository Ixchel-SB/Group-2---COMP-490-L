using UnityEngine;
using TMPro;
using System.Collections;

public class ArrowInteraction : MonoBehaviour
{
    [Header("Vendor Settings")]
    public GameObject vendorNPC; // Drag Vendor_SittingIdle here
    public VendorDialogueSystem vendorDialogueSystem; // Drag VendorDialogueManager here
    public Transform vendorCameraPos; // Drag VendorCameraPos here
    
    [Header("Interaction")]
    public string interactionMessage = "Press F to talk to vendor";
    public GameObject interactionPrompt; // UI text prompt
    
    private bool playerInRange = false;
    private bool hasInteracted = false;
    private CanvasGroup promptCanvasGroup;
    private GameObject player;
    private MonoBehaviour playerController;
    
    void Start()
    {
        // Setup interaction prompt
        if (interactionPrompt != null)
        {
            promptCanvasGroup = interactionPrompt.GetComponent<CanvasGroup>();
            if (promptCanvasGroup == null)
                promptCanvasGroup = interactionPrompt.AddComponent<CanvasGroup>();
            promptCanvasGroup.alpha = 0f;
            interactionPrompt.SetActive(false);
        }
        
        // Find player and controller
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<MonoBehaviour>();
            if (playerController == null)
            {
                Transform playerArmature = player.transform.Find("PlayerArmature");
                if (playerArmature != null)
                    playerController = playerArmature.GetComponent<MonoBehaviour>();
            }
        }
        
        // Arrow starts hidden - PlazaTrigger will reveal it
        gameObject.SetActive(false);
    }
    
    void Update()
    {
        // Check for interaction when player is in range
        if (playerInRange && !hasInteracted && Input.GetKeyDown(KeyCode.F))
        {
            InteractWithVendor();
        }
    }
    
    void InteractWithVendor()
    {
        hasInteracted = true;
        Debug.Log("Player interacted with arrow - starting vendor dialogue");
        
        // Hide interaction prompt immediately
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        
        // Hide the arrow IMMEDIATELY when dialogue starts
        gameObject.SetActive(false);
        Debug.Log("Arrow disappeared!");
        
        // Start vendor dialogue
        if (vendorDialogueSystem != null)
        {
            // Set the camera position for vendor dialogue
            vendorDialogueSystem.dialogueCameraPosition = vendorCameraPos;
            
            // Start the dialogue
            vendorDialogueSystem.StartDialogue();
        }
        else
        {
            Debug.LogError("VendorDialogueSystem not assigned!");
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
