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
    
    private int currentLine = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    
    private GameObject player;
    private MonoBehaviour playerController;
    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;
    private bool hasTalked = false;
    
    void Start()
    {
        Time.timeScale = 1f;
        
        dialoguePanel.SetActive(false);
        if (continueText != null)
            continueText.text = "Press F to continue";
        
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
        
        if (playerController != null)
            playerController.enabled = true;
        if (playerFollowCamera != null)
            playerFollowCamera.enabled = true;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPos;
            mainCamera.transform.rotation = originalCameraRot;
        }
        
        hasTalked = true;
        
        DormManager dormManager = FindObjectOfType<DormManager>();
        if (dormManager != null)
        {
            dormManager.RoommateTalked(roommateName);
        }
        
        Debug.Log(roommateName + " dialogue ended");
    }
}
