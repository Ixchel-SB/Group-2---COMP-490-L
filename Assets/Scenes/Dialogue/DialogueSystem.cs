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
    public TextMeshProUGUI continueText;
    
    public List<DialogueLine> dialogueLines;
    public float typingSpeed = 0.01f;
    
    [Header("Camera Settings")]
    public Camera mainCamera;
    public Transform dialogueCameraPosition;
    public MonoBehaviour playerFollowCamera;
    
    [Header("Transition Settings")]
    public GameObject blackScreenPanel;
    public float fadeDuration = 0.5f;
    
    private int currentLine = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    
    private GameObject player;
    private GameObject playerModel;
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
        
        // Initialize black screen panel
        if (blackScreenPanel != null)
        {
            CanvasGroup cg = blackScreenPanel.GetComponent<CanvasGroup>();
            if (cg == null) cg = blackScreenPanel.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
        
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            // Find the player's visible model
            SkinnedMeshRenderer skinnedMesh = player.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMesh != null)
            {
                playerModel = skinnedMesh.gameObject;
            }
            else
            {
                playerModel = player;
            }
            
            playerController = player.GetComponent<MonoBehaviour>();
            
            if (playerController == null)
            {
                Transform playerArmature = player.transform.Find("PlayerArmature");
                if (playerArmature != null)
                {
                    playerController = playerArmature.GetComponent<MonoBehaviour>();
                }
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
    
    public void StartDialogue()
    {
        currentLine = 0;
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        
        // HIDE PLAYER CHARACTER
        if (playerModel != null)
        {
            playerModel.SetActive(false);
            Debug.Log("Player character hidden");
        }
        
        // Freeze player movement
        if (playerController != null) playerController.enabled = false;
        if (playerFollowCamera != null) playerFollowCamera.enabled = false;
        
        if (mainCamera != null && dialogueCameraPosition != null)
        {
            originalCameraPos = mainCamera.transform.position;
            originalCameraRot = mainCamera.transform.rotation;
            mainCamera.transform.position = dialogueCameraPosition.position;
            mainCamera.transform.rotation = dialogueCameraPosition.rotation;
        }
        
        DisplayLine();
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
        if (currentLine == dialogueLines.Count - 1)
        {
            EndDialogue();
            return;
        }
        currentLine++;
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
        StartCoroutine(TransitionSequence());
    }
    
    IEnumerator TransitionSequence()
    {
        if (blackScreenPanel == null)
        {
            Debug.LogError("BlackScreenPanel not assigned!");
            yield break;
        }
        
        CanvasGroup cg = blackScreenPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = blackScreenPanel.AddComponent<CanvasGroup>();
        
        // FADE TO BLACK
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        cg.alpha = 1f;
        
        Debug.Log("Screen black - waiting 5 seconds");
        
        // WAIT 5 SECONDS
        yield return new WaitForSeconds(5f);
        
        Debug.Log("5 seconds passed - restoring game");
        
        // REMOVE NUN
        NPCInteraction npc = FindObjectOfType<NPCInteraction>();
        if (npc != null)
        {
            npc.gameObject.SetActive(false);
            Debug.Log("Nun removed");
        }
        
        // RESTORE CONTROLS
        if (playerController != null)
        {
            playerController.enabled = true;
            Debug.Log("Player controls restored");
        }
        
        if (playerFollowCamera != null)
        {
            playerFollowCamera.enabled = true;
            Debug.Log("Camera follow restored");
        }
        
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPos;
            mainCamera.transform.rotation = originalCameraRot;
            Debug.Log("Camera position restored");
        }
        
        // SHOW PLAYER CHARACTER AGAIN
        if (playerModel != null)
        {
            playerModel.SetActive(true);
            Debug.Log("Player character shown again");
        }
        
        // NOTIFY GAME PROGRESS MANAGER THAT NUN DIALOGUE IS COMPLETE
        GameProgressManager progressManager = FindObjectOfType<GameProgressManager>();
        if (progressManager != null)
        {
            progressManager.CompleteNunDialogue();
            Debug.Log("Nun dialogue completed - notified GameProgressManager");
        }
        else
        {
            Debug.LogWarning("GameProgressManager not found in scene! Make sure it exists.");
        }
        
        // SHOW MAP PROMPT
        ShowMapPrompt();
        
        // FADE BACK IN
        Debug.Log("Starting fade back in");
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        cg.alpha = 0f;
        
        Debug.Log("Fade back complete - screen is normal");
    }
    
    void ShowMapPrompt()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        
        GameObject textObj = new GameObject("MapPromptText");
        textObj.transform.SetParent(canvas.transform, false);
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "Press M for Map";
        tmp.fontSize = 36;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        
        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(400, 100);
        
        CanvasGroup cg = textObj.AddComponent<CanvasGroup>();
        cg.alpha = 0;
        StartCoroutine(FadePrompt(cg, textObj));
    }
    
    IEnumerator FadePrompt(CanvasGroup cg, GameObject textObj)
    {
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed);
            yield return null;
        }
        
        yield return new WaitForSeconds(3f);
        
        elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed);
            yield return null;
        }
        
        Destroy(textObj);
    }
}

