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
    public TextMeshProUGUI continueText; //"Press F to continue"
    
    public List<DialogueLine> dialogueLines;
    public float typingSpeed = 0.01f; //Changed typing speed!
    
    [Header("Camera Settings")]
    public Camera mainCamera;
    public Transform dialogueCameraPosition; //Where camera moves during dialogue
    public MonoBehaviour playerFollowCamera; //The camera follow script to disable
    
    [Header("Transition Settings")]
    public Image blackScreenImage; //Drag a full-screen black UI Image here
    public float transitionDuration = 0.10f; //How long the black flash lasts
    
    private int currentLine = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    
    //Player references
    private GameObject player;
    private MonoBehaviour playerController;
    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;
    
    void Start()
    {
        dialoguePanel.SetActive(false);
        if (continueText != null)
        {
            continueText.text = "Press F to continue";
            continueText.gameObject.SetActive(false);
        }
        
        // Make sure black screen is invisible at start
        if (blackScreenImage != null)
        {
            Color c = blackScreenImage.color;
            c.a = 0f;
            blackScreenImage.color = c;
        }
        
        // Find the player
        player = GameObject.FindGameObjectWithTag("Player");
        
        // Find ANY movement script on the player
        if (player != null)
        {
            playerController = player.GetComponent<MonoBehaviour>();
            
            if (playerController == null)
            {
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
        Debug.Log("Displaying line " + currentLine + " of " + dialogueLines.Count);
        
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        
        speakerText.text = dialogueLines[currentLine].speakerName + ":";
        typingCoroutine = StartCoroutine(TypeLine(dialogueLines[currentLine].line));
    }
    
    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        
        if (continueText != null)
            continueText.gameObject.SetActive(false);
        
        float currentSpeed = typingSpeed; // Store the current typing speed
        Debug.Log("Typing with speed: " + currentSpeed); // Debug to confirm speed
        
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(currentSpeed);
        }
        
        isTyping = false;
        
        if (continueText != null)
            continueText.gameObject.SetActive(true);
    }
    
    void NextLine()
    {
        // If this is the last line, end dialogue
        if (currentLine == dialogueLines.Count - 1)
        {
            Debug.Log("Last line completed - ending dialogue");
            EndDialogue();
            return;
        }
        
        // Otherwise, move to next line
        currentLine++;
        Debug.Log("Moving to line " + currentLine);
        DisplayLine();
    }
    
    void EndDialogue()
    {
        Debug.Log("EndDialogue called - cleaning up");
        
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        
        if (continueText != null)
            continueText.gameObject.SetActive(false);
        
        // Start the black screen transition
        StartCoroutine(TransitionAndRestore());
    }
    
    IEnumerator TransitionAndRestore()
    {
        // Flash to black
        if (blackScreenImage != null)
        {
            float elapsed = 0f;
            Color c = blackScreenImage.color;
            
            // Fade to black
            while (elapsed < transitionDuration / 2)
            {
                elapsed += Time.unscaledDeltaTime;
                c.a = Mathf.Lerp(0f, 1f, elapsed / (transitionDuration / 2));
                blackScreenImage.color = c;
                yield return null;
            }
            
            c.a = 1f;
            blackScreenImage.color = c;
        }
        
        // Restore everything during black screen
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
            Debug.Log("Camera restored to original position");
        }
        
        // Fade back in
        if (blackScreenImage != null)
        {
            float elapsed = 0f;
            Color c = blackScreenImage.color;
            
            while (elapsed < transitionDuration / 2)
            {
                elapsed += Time.unscaledDeltaTime;
                c.a = Mathf.Lerp(1f, 0f, elapsed / (transitionDuration / 2));
                blackScreenImage.color = c;
                yield return null;
            }
            
            c.a = 0f;
            blackScreenImage.color = c;
        }
        
        // Notify the NPC that dialogue ended (this will disable further interaction)
        NPCInteraction npc = FindObjectOfType<NPCInteraction>();
        if (npc != null)
        {
            npc.OnDialogueEnd();
            Debug.Log("Notified NPC that dialogue ended - NPC permanently disabled");
        }
        
        Debug.Log("Dialogue ended - Player can now move normally");
    }
}
