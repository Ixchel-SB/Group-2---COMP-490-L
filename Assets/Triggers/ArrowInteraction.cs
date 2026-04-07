using UnityEngine;
using TMPro;
using System.Collections;

public class ArrowInteraction : MonoBehaviour
{
    [Header("Vendor Settings")]
    public GameObject vendorNPC;
    public VendorDialogueSystem vendorDialogueSystem;
    public Transform vendorCameraPos;
    
    [Header("Interaction")]
    public string interactionMessage = "Press F to talk to vendor";
    public GameObject interactionPrompt;
    
    private bool playerInRange = false;
    private bool hasInteracted = false;
    private CanvasGroup promptCanvasGroup;
    private GameObject player;
    private MonoBehaviour playerController;
    
    void OnEnable()
    {
        Debug.Log("ArrowInteraction: Arrow was ENABLED at " + Time.time);
    }
    
    void OnDisable()
    {
        Debug.Log("ArrowInteraction: Arrow was DISABLED at " + Time.time);
    }
    
    void Start()
    {
        Debug.Log("ArrowInteraction Start - Arrow active: " + gameObject.activeSelf);
        
        if (interactionPrompt != null)
        {
            promptCanvasGroup = interactionPrompt.GetComponent<CanvasGroup>();
            if (promptCanvasGroup == null)
                promptCanvasGroup = interactionPrompt.AddComponent<CanvasGroup>();
            promptCanvasGroup.alpha = 0f;
            interactionPrompt.SetActive(false);
        }
        
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
        
        // REMOVED: gameObject.SetActive(false);
        // The arrow should be disabled by PlazaTriggerText at start, not here
        Debug.Log("ArrowInteraction: Ready - arrow active: " + gameObject.activeSelf);
    }
    
    void Update()
    {
        if (playerInRange && !hasInteracted && Input.GetKeyDown(KeyCode.F))
        {
            InteractWithVendor();
        }
    }
    
    void InteractWithVendor()
    {
        hasInteracted = true;
        Debug.Log("Player interacted with arrow - starting vendor dialogue");
        
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        
        gameObject.SetActive(false);
        Debug.Log("Arrow disappeared after interaction!");
        
        if (vendorDialogueSystem != null)
        {
            vendorDialogueSystem.dialogueCameraPosition = vendorCameraPos;
            vendorDialogueSystem.StartDialogue();
            Debug.Log("Vendor dialogue started");
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
            Debug.Log("Player in range of arrow");
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
            Debug.Log("Player left range of arrow");
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
