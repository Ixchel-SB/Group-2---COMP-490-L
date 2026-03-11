using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public string npcName = "Sister Mary";
    public DialogueSystem dialogueSystem;
    public GameObject interactionPrompt; // "Press F to talk" UI
    
    private bool playerInRange = false;
    private bool dialogueActive = false;
    
    void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    void Update()
    {
        // Only allow starting dialogue if not already in dialogue
        if (playerInRange && !dialogueActive && Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }
    
    void Interact()
    {
        Debug.Log("Starting dialogue with " + npcName);
        dialogueActive = true;
        
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
        dialogueActive = false;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // Only show prompt if not in dialogue
            if (!dialogueActive && interactionPrompt != null)
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