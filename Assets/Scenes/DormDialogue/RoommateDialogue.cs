using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class RoommateDialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        [TextArea(2, 4)]
        public string line;
    }
    
    public string roommateName = "Valentina";
    public List<DialogueLine> dialogueLines;
    public float typingSpeed = 0.05f;
    
    [Header("Camera Settings")]
    public Camera mainCamera;
    public Transform dialogueCameraPosition;
    public MonoBehaviour playerFollowCamera;
    
    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI continueText;
    
    [Header("Transition Settings")]
    public GameObject blackScreenPanel;
    public float fadeDuration = 0.5f;
    public float waitAfterDialogue = 1f;
    
    private int currentLine = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    
    private GameObject player;
    private GameObject playerModel;
    private MonoBehaviour playerController;
    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;
    private bool hasTalked = false;
    private CanvasGroup blackCanvasGroup;
    private bool isTransitioning = false;
    
    void Start()
    {
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
        
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            SkinnedMeshRenderer skinnedMesh = player.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMesh != null)
                playerModel = skinnedMesh.gameObject;
            else
                playerModel = player;
            
            playerController = player.GetComponent<MonoBehaviour>();
            if (playerController == null)
            {
                Transform playerArmature = player.transform.Find("PlayerArmature");
                if (playerArmature != null)
                    playerController = playerArmature.GetComponent<MonoBehaviour>();
            }
        }
    }
    
    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.F) && !isTyping)
        {
            NextLine();
        }
    }
    
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
    
    public void StartDialogue()
    {
        if (hasTalked) return;
        
        currentLine = 0;
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        
        if (playerModel != null)
            playerModel.SetActive(false);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (playerController != null)
            playerController.enabled = false;
        if (playerFollowCamera != null)
            playerFollowCamera.enabled = false;
        
        if (mainCamera != null && dialogueCameraPosition != null)
        {
            originalCameraPos = mainCamera.transform.position;
            originalCameraRot = mainCamera.transform.rotation;
            mainCamera.transform.position = dialogueCameraPosition.position;
            mainCamera.transform.rotation = dialogueCameraPosition.rotation;
        }
        
        DisplayLine();
    }
    
    public void SetAsSecondDialogue()
    {
        hasTalked = false;
        Debug.Log(roommateName + "'s second dialogue is now available");
    }
    
    void DisplayLine()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        
        speakerText.text = dialogueLines[currentLine].speakerName + ":";
        typingCoroutine = StartCoroutine(TypeLine(dialogueLines[currentLine].line));
    }
    
    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        if (continueText != null) continueText.gameObject.SetActive(false);
        
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
        if (continueText != null) continueText.gameObject.SetActive(true);
    }
    
    void NextLine()
    {
        currentLine++;
        
        if (currentLine < dialogueLines.Count)
        {
            DisplayLine();
        }
        else
        {
            EndDialogue();
        }
    }
    
    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        if (continueText != null) continueText.gameObject.SetActive(false);
        
        StartCoroutine(TransitionSequence());
    }
    
    IEnumerator TransitionSequence()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;
        
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
        
        Debug.Log("Screen black - processing end of dialogue");
        
        yield return new WaitForSecondsRealtime(waitAfterDialogue);
        
        if (playerController != null)
            playerController.enabled = true;
        if (playerFollowCamera != null)
            playerFollowCamera.enabled = true;
        
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPos;
            mainCamera.transform.rotation = originalCameraRot;
        }
        
        if (playerModel != null)
            playerModel.SetActive(true);
        
        hasTalked = true;
        
        DormManager dormManager = FindObjectOfType<DormManager>();
        if (dormManager != null)
        {
            dormManager.RoommateTalked(roommateName);
            
            if (roommateName == "Valentina")
            {
                dormManager.ValentinaFirstDialogueComplete();
            }
        }
        
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
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        isTransitioning = false;
        Debug.Log(roommateName + " dialogue ended");
    }
}
