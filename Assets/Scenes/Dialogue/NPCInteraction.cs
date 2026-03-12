using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public string npcName = "Sister Mary";
    public DialogueSystem dialogueSystem;
    public GameObject interactionPrompt; // "Press F to talk" UI
    
    private bool playerInRange = false;
    private bool dialogueActive = false;
    private bool hasInteracted = false; // New flag for one-time interaction
    
    void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    void Update()
    {
        // Only allow starting dialogue if:
        // - Player in range
        // - Not already in dialogue
        // - Has NOT interacted before (one-time only)
        if (playerInRange && !dialogueActive && !hasInteracted && Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }
    
    void Interact()
    {
        Debug.Log("Starting dialogue with " + npcName);
        dialogueActive = true;
        hasInteracted = true; // Mark as interacted - won't be able to talk again
        
        if (dialogueSystem != null)
        {
            dialogueSystem.StartDialogue();
        }
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    // This will be called by DialogueSystem when dialogue ends
    public void OnDialogueEnd()
    {
        Debug.Log("NPC: Dialogue ended callback received - permanently disabled");
        dialogueActive = false;
        
        // Permanently hide prompt
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        
        // Optional: Disable the collider so player can't even trigger
        // GetComponent<Collider>().enabled = false;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // Only show prompt if not in dialogue and hasn't interacted yet
            if (!dialogueActive && !hasInteracted && interactionPrompt != null)
                interactionPrompt.SetActive(true);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
        }
    }
}
