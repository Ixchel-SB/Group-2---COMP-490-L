using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueSystem : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        [TextArea(2, 4)]
        public string line;
    }
    
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI continueText; // "Press F to continue"
    
    public List<DialogueLine> dialogueLines;
    public float typingSpeed = 0.05f;
    
    [Header("Camera Settings")]
    public Camera mainCamera;
    public Transform dialogueCameraPosition; // Where camera moves during dialogue
    public MonoBehaviour playerFollowCamera; // The camera follow script to disable
    
    private int currentLine = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    
    // Player references
    private GameObject player;
    private MonoBehaviour playerController;
    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;
    
    void Start()
    {
        dialoguePanel.SetActive(false);
        if (continueText != null)
            continueText.text = "Press F to continue";
        
        // Find the player
        player = GameObject.FindGameObjectWithTag("Player");
        
        // Find ANY movement script on the player
        if (player != null)
        {
            // Check common controller types
            playerController = player.GetComponent<MonoBehaviour>();
            
            // If not on root, check children
            if (playerController == null)
            {
                // Look for PlayerArmature child
                Transform playerArmature = player.transform.Find("PlayerArmature");
                if (playerArmature != null)
                {
                    playerController = playerArmature.GetComponent<MonoBehaviour>();
                }
            }
            
            Debug.Log("Player controller found: " + (playerController != null ? playerController.GetType().Name : "None"));
        }
    }
    
    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.F) && !isTyping)
        {
            NextLine();
        }
    }
    
    public void StartDialogue()
    {
        currentLine = 0;
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        
        // Freeze player movement
        if (playerController != null)
        {
            playerController.enabled = false;
            Debug.Log("Player controller disabled");
        }
        
        // Disable follow camera
        if (playerFollowCamera != null)
        {
            playerFollowCamera.enabled = false;
            Debug.Log("Follow camera disabled");
        }
        
        // Store camera position and move to dialogue view
        if (mainCamera != null && dialogueCameraPosition != null)
        {
            originalCameraPos = mainCamera.transform.position;
            originalCameraRot = mainCamera.transform.rotation;
            
            mainCamera.transform.position = dialogueCameraPosition.position;
            mainCamera.transform.rotation = dialogueCameraPosition.rotation;
            Debug.Log("Camera moved to dialogue position");
        }
        
        DisplayLine();
    }
    
    void DisplayLine()
    {
        if (currentLine < dialogueLines.Count)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            
            speakerText.text = dialogueLines[currentLine].speakerName + ":";
            typingCoroutine = StartCoroutine(TypeLine(dialogueLines[currentLine].line));
        }
        else
        {
            EndDialogue();
        }
    }
    
    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
    }
    
    void NextLine()
    {
        currentLine++;
        DisplayLine();
    }
    
    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        
        // Unfreeze player movement
        if (playerController != null)
        {
            playerController.enabled = true;
            Debug.Log("Player controller enabled");
        }
        
        // Re-enable follow camera
        if (playerFollowCamera != null)
        {
            playerFollowCamera.enabled = true;
            Debug.Log("Follow camera enabled");
        }
        
        // Restore camera position
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPos;
            mainCamera.transform.rotation = originalCameraRot;
            Debug.Log("Camera restored");
        }
        
        // Notify the NPC that dialogue ended
        FindObjectOfType<NPCInteraction>()?.OnDialogueEnd();
        
        // Optional: Trigger map give event
        Debug.Log("Dialogue ended - map should be given here");
    }
}
