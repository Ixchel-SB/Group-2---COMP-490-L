using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public string npcName = "Sister Mary";
    public DialogueSystem dialogueSystem;
    public GameObject interactionPrompt; // "Press F to talk" UI
    
    private bool playerInRange = false;
    
    void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }
    
    void Interact()
    {
        Debug.Log("Talking to " + npcName);
        
        if (dialogueSystem != null)
        {
            dialogueSystem.StartDialogue();
        }
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionPrompt != null)
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