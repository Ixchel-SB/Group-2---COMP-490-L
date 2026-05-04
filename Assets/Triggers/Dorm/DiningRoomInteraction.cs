using UnityEngine;
using TMPro;
using System.Collections;

public class DiningRoomInteraction : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI thinkingText;
    public GameObject blackScreenPanel;
    
    [Header("Dialogue UI Elements")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI continueText;
    
    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.F;
    public float interactionRange = 3f;
    
    [Header("Doors to Lock")]
    public GameObject dormDoor;
    public GameObject entranceDoor;
    public Transform outsideDormPosition;
    private Collider dormDoorCollider;
    private Collider entranceDoorCollider;
    private MonoBehaviour dormDoorScript;
    private MonoBehaviour entranceDoorScript;
    
    [Header("Character Models - OLD (to disable)")]
    public GameObject oldMarcelo;
    public GameObject oldElio;
    
    [Header("Character Models - NEW (to enable AFTER closet interaction)")]
    public GameObject newMarcelo;
    public GameObject newElio;
    public GameObject newValentina;
    
    [Header("Samael Model")]
    public GameObject samaelModel;  // Drunk Idle Variation - only appears during Samael dialogue
    
    [Header("Samael Dialogue Camera")]
    public Transform samaelCamPos;
    private Camera mainCamera;
    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;
    
    [Header("Thinking Texts")]
    public string doorThinkingText = "Should hurry up before the food is gone";
    public float doorThinkingDuration = 2f;
    
    public string hurryText = "Press F to sit at the table";
    
    [Header("Story Text")]
    [TextArea(5, 10)]
    public string storyText = "Metzly and her roommates ate their dinner and decided to go to the cemetery tomorrow before school starts, hoping this would clear Metzly's mind. Once they all finished, Metzly and Valentina took a bath and got all their supplies ready for school while the boys procrastinated the whole day. The girls were able to go to bed early.";
    
    [Header("Time Text")]
    public string timeText = "Monday 6:15am";
    
    [Header("Thinking After")]
    public string thinkingAfterText = "I should head to the cemetery... I'm sure everyone else headed there by now.";
    public float thinkingAfterDuration = 4f;
    
    [Header("Samael Dialogue Lines")]
    public string[] samaelDialogueLines = new string[]
    {
        "HEY!",
        "Hey! Are you listening! *burp*",
        "The stupid nun sent me to wake you all up. So get up!",
        "*burp* *spits on floor*",
        "Good for you. *burp*"
    };
    
    public string[] metzlyDialogueLines = new string[]
    {
        "*Gasp*",
        "Yes",
        "Umm... We're already woke up."
    };
    
    private CanvasGroup thinkingCanvasGroup;
    private CanvasGroup blackCanvasGroup;
    private bool hasInteracted = false;
    private bool isSequenceRunning = false;
    private bool waitingForF = false;
    private bool playerInTrigger = false;
    private bool closetSequenceCompleted = false;
    private bool chairInteractionDone = false;
    private bool samaelDialogueDone = false;
    
    private GameObject player;
    private MonoBehaviour playerController;
    private PostPhotoSequence postPhotoSequence;
    private GameObject playerModel;
    
    void Start()
    {
        Debug.Log("=== DINING ROOM INTERACTION START ===");
        
        mainCamera = Camera.main;
        postPhotoSequence = FindObjectOfType<PostPhotoSequence>();
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
            
            SkinnedMeshRenderer skinnedMesh = player.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMesh != null)
                playerModel = skinnedMesh.gameObject;
        }
        
        if (thinkingText != null)
        {
            thinkingCanvasGroup = thinkingText.GetComponent<CanvasGroup>();
            if (thinkingCanvasGroup == null)
                thinkingCanvasGroup = thinkingText.gameObject.AddComponent<CanvasGroup>();
            thinkingCanvasGroup.alpha = 0f;
            thinkingText.gameObject.SetActive(false);
            
            thinkingText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 800f);
            thinkingText.enableWordWrapping = true;
        }
        
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (continueText != null)
            continueText.text = "Press F to continue";
        
        if (blackScreenPanel != null)
        {
            blackCanvasGroup = blackScreenPanel.GetComponent<CanvasGroup>();
            if (blackCanvasGroup == null)
                blackCanvasGroup = blackScreenPanel.AddComponent<CanvasGroup>();
            blackCanvasGroup.alpha = 0f;
        }
        
        if (dormDoor != null)
        {
            dormDoorCollider = dormDoor.GetComponent<Collider>();
            dormDoorScript = dormDoor.GetComponent<MonoBehaviour>();
        }
        
        if (entranceDoor != null)
        {
            entranceDoorCollider = entranceDoor.GetComponent<Collider>();
            entranceDoorScript = entranceDoor.GetComponent<MonoBehaviour>();
        }
        
        if (newMarcelo != null) newMarcelo.SetActive(false);
        if (newElio != null) newElio.SetActive(false);
        if (newValentina != null) newValentina.SetActive(false);
        
        // Samael model starts DISABLED - only appears during dialogue
        if (samaelModel != null) samaelModel.SetActive(false);
        
        if (oldMarcelo != null) oldMarcelo.SetActive(true);
        if (oldElio != null) oldElio.SetActive(true);
        
        LockEntranceDoor();
        
        Debug.Log("=== DINING ROOM INTERACTION READY ===");
        Debug.Log($"SamaelCamPos assigned: {samaelCamPos != null}");
    }
    
    void LockEntranceDoor()
    {
        if (entranceDoor != null)
        {
            if (entranceDoorCollider != null) entranceDoorCollider.enabled = false;
            if (entranceDoorScript != null) entranceDoorScript.enabled = false;
        }
    }
    
    void UnlockEntranceDoor()
    {
        if (entranceDoor != null)
        {
            if (entranceDoorCollider != null) entranceDoorCollider.enabled = true;
            if (entranceDoorScript != null) entranceDoorScript.enabled = true;
        }
    }
    
    void Update()
    {
        if (!closetSequenceCompleted && postPhotoSequence != null)
        {
            if (postPhotoSequence.HasArrowPressed())
            {
                closetSequenceCompleted = true;
                EnableNewModels();
                DisableOldModels();
            }
        }
        
        if (waitingForF && Input.GetKeyDown(KeyCode.F))
        {
            waitingForF = false;
        }
        
        if (!hasInteracted && !isSequenceRunning && playerInTrigger && closetSequenceCompleted && !chairInteractionDone && Input.GetKeyDown(interactKey))
        {
            StartCoroutine(RunSequence());
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            player = other.gameObject;
            
            if (closetSequenceCompleted && !chairInteractionDone)
            {
                if (thinkingText != null)
                {
                    thinkingText.gameObject.SetActive(true);
                    thinkingText.text = hurryText;
                    thinkingCanvasGroup.alpha = 1f;
                }
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            player = null;
            if (thinkingText != null) thinkingText.gameObject.SetActive(false);
        }
    }
    
    void EnableNewModels()
    {
        if (newMarcelo != null) newMarcelo.SetActive(true);
        if (newElio != null) newElio.SetActive(true);
        if (newValentina != null) newValentina.SetActive(true);
    }
    
    void DisableOldModels()
    {
        if (oldMarcelo != null) oldMarcelo.SetActive(false);
        if (oldElio != null) oldElio.SetActive(false);
    }
    
    void RemoveDiningRoomModels()
    {
        if (newMarcelo != null) newMarcelo.SetActive(false);
        if (newElio != null) newElio.SetActive(false);
        if (newValentina != null) newValentina.SetActive(false);
    }
    
    void LockDoors()
    {
        if (dormDoor != null)
        {
            if (dormDoorCollider != null) dormDoorCollider.enabled = false;
            if (dormDoorScript != null) dormDoorScript.enabled = false;
        }
    }
    
    void UnlockDoors()
    {
        if (dormDoor != null)
        {
            if (dormDoorCollider != null) dormDoorCollider.enabled = true;
            if (dormDoorScript != null) dormDoorScript.enabled = true;
        }
    }
    
    void FreezePlayer()
    {
        if (playerController != null) playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    void UnfreezePlayer()
    {
        if (playerController != null) playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void HidePlayerModel()
    {
        if (playerModel != null) playerModel.SetActive(false);
        if (player != null) player.SetActive(false);
        Debug.Log("Player model hidden");
    }
    
    void ShowPlayerModel()
    {
        if (player != null) player.SetActive(true);
        if (playerModel != null) playerModel.SetActive(true);
        Debug.Log("Player model shown");
    }
    
    void MoveCameraToSamaelPosition()
    {
        if (mainCamera != null && samaelCamPos != null)
        {
            originalCameraPos = mainCamera.transform.position;
            originalCameraRot = mainCamera.transform.rotation;
            
            mainCamera.transform.position = samaelCamPos.position;
            mainCamera.transform.rotation = samaelCamPos.rotation;
            Debug.Log($"Camera moved to Samael position: {samaelCamPos.position}");
        }
        else
        {
            Debug.LogError($"Cannot move camera! mainCamera={mainCamera != null}, samaelCamPos={samaelCamPos != null}");
        }
    }
    
    void RestoreCameraPosition()
    {
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPos;
            mainCamera.transform.rotation = originalCameraRot;
            Debug.Log("Camera restored");
        }
    }
    
    public void ShowDoorThinkingText()
    {
        if (!chairInteractionDone) StartCoroutine(ShowDoorThinkingTextCoroutine());
    }
    
    public bool IsChairInteractionDone()
    {
        return chairInteractionDone;
    }
    
    public bool IsSamaelDialogueDone()
    {
        return samaelDialogueDone;
    }
    
    public void StartSamaelDialogue()
    {
        Debug.Log($"StartSamaelDialogue called - samaelDialogueDone={samaelDialogueDone}, chairInteractionDone={chairInteractionDone}");
        if (!samaelDialogueDone && chairInteractionDone)
        {
            Debug.Log("Starting Samael dialogue coroutine...");
            StartCoroutine(SamaelDialogueSequence());
        }
        else
        {
            Debug.LogWarning($"Cannot start Samael dialogue: samaelDialogueDone={samaelDialogueDone}, chairInteractionDone={chairInteractionDone}");
        }
    }
    
    IEnumerator ShowDoorThinkingTextCoroutine()
    {
        if (thinkingText != null)
        {
            thinkingText.gameObject.SetActive(true);
            thinkingText.text = doorThinkingText;
            thinkingCanvasGroup.alpha = 1f;
            yield return new WaitForSeconds(doorThinkingDuration);
            
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                thinkingCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
                yield return null;
            }
            thinkingCanvasGroup.alpha = 0f;
            thinkingText.gameObject.SetActive(false);
        }
    }
    
    IEnumerator ShowDialogue(string line, string speaker)
    {
        if (dialoguePanel == null) yield break;
        
        dialoguePanel.SetActive(true);
        speakerText.text = speaker + ":";
        dialogueText.text = "";
        
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        
        if (continueText != null) continueText.gameObject.SetActive(true);
        
        waitingForF = true;
        yield return new WaitUntil(() => !waitingForF);
        
        if (continueText != null) continueText.gameObject.SetActive(false);
        dialoguePanel.SetActive(false);
    }
    
    IEnumerator SamaelDialogueSequence()
    {
        Debug.Log("=== SAMAEL DIALOGUE SEQUENCE STARTED ===");
        
        samaelDialogueDone = false;
        
        // Show Samael model before dialogue starts
        if (samaelModel != null)
        {
            samaelModel.SetActive(true);
            Debug.Log("Samael model activated");
        }
        
        Debug.Log("Freezing player...");
        FreezePlayer();
        
        Debug.Log("Hiding player model...");
        HidePlayerModel();
        
        Debug.Log("Moving camera to Samael position...");
        MoveCameraToSamaelPosition();
        
        yield return new WaitForSeconds(0.2f);
        
        Debug.Log("Starting dialogue...");
        yield return StartCoroutine(ShowDialogue(samaelDialogueLines[0], "Samael"));
        yield return StartCoroutine(ShowDialogue(metzlyDialogueLines[0], "Metzly"));
        yield return StartCoroutine(ShowDialogue(samaelDialogueLines[1], "Samael"));
        yield return StartCoroutine(ShowDialogue(metzlyDialogueLines[1], "Metzly"));
        yield return StartCoroutine(ShowDialogue(metzlyDialogueLines[2], "Metzly"));
        yield return StartCoroutine(ShowDialogue(samaelDialogueLines[2], "Samael"));
        yield return StartCoroutine(ShowDialogue(metzlyDialogueLines[2], "Metzly"));
        yield return StartCoroutine(ShowDialogue(samaelDialogueLines[3], "Samael"));
        yield return StartCoroutine(ShowDialogue(samaelDialogueLines[4], "Samael"));
        
        Debug.Log("Fading to black...");
        if (blackCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                blackCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
                yield return null;
            }
            blackCanvasGroup.alpha = 1f;
        }
        
        Debug.Log("Removing Samael model...");
        if (samaelModel != null)
        {
            samaelModel.SetActive(false);
            Debug.Log("Samael model removed");
        }
        
        Debug.Log("Fading back...");
        if (blackCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                blackCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
                yield return null;
            }
            blackCanvasGroup.alpha = 0f;
        }
        
        Debug.Log("Restoring camera...");
        RestoreCameraPosition();
        
        Debug.Log("Showing player model...");
        ShowPlayerModel();
        
        Debug.Log("Unfreezing player...");
        UnfreezePlayer();
        
        samaelDialogueDone = true;
        
        Debug.Log("=== SAMAEL DIALOGUE SEQUENCE COMPLETED ===");
    }
    
    IEnumerator ShowThinkingText(string message, float duration)
    {
        if (thinkingText == null) yield break;
        
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = message;
        
        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            thinkingCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
            yield return null;
        }
        thinkingCanvasGroup.alpha = 1f;
        
        yield return new WaitForSeconds(duration);
        
        elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            thinkingCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
            yield return null;
        }
        thinkingCanvasGroup.alpha = 0f;
        thinkingText.gameObject.SetActive(false);
    }
    
    IEnumerator RunSequence()
    {
        Debug.Log("=== CHAIR SEQUENCE STARTED ===");
        
        hasInteracted = true;
        isSequenceRunning = true;
        
        if (thinkingText != null) thinkingText.gameObject.SetActive(false);
        
        LockDoors();
        FreezePlayer();
        
        if (blackCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                blackCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
                yield return null;
            }
            blackCanvasGroup.alpha = 1f;
        }
        
        if (thinkingText != null)
        {
            thinkingText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 800f);
            thinkingText.fontSize = 24;
            
            thinkingText.gameObject.SetActive(true);
            thinkingText.text = storyText;
            thinkingCanvasGroup.alpha = 1f;
            yield return new WaitForSeconds(8f);
            thinkingText.gameObject.SetActive(false);
        }
        
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = timeText;
        thinkingCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(3f);
        thinkingText.gameObject.SetActive(false);
        
        if (blackCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                blackCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
                yield return null;
            }
            blackCanvasGroup.alpha = 0f;
        }
        
        yield return StartCoroutine(ShowThinkingText(thinkingAfterText, thinkingAfterDuration));
        
        RemoveDiningRoomModels();
        UnlockDoors();
        UnlockEntranceDoor();
        UnfreezePlayer();
        isSequenceRunning = false;
        chairInteractionDone = true;
        
        Debug.Log("=== CHAIR SEQUENCE COMPLETED - chairInteractionDone = true ===");
        
        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null) triggerCollider.enabled = false;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
        if (samaelCamPos != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(samaelCamPos.position, 0.3f);
            Gizmos.DrawLine(samaelCamPos.position, samaelCamPos.position + samaelCamPos.forward * 2f);
        }
    }
}
