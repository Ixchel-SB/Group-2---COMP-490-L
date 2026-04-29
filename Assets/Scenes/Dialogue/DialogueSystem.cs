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
        
        if (blackScreenPanel != null)
        {
            CanvasGroup cg = blackScreenPanel.GetComponent<CanvasGroup>();
            if (cg == null) cg = blackScreenPanel.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
        }
        
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
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
    
    // ADD THIS METHOD - Used by PauseManager to check if dialogue is active
    public bool IsDialogueActive()
    {
        return isDialogueActive;
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
        
        // Freeze player movement ONLY - NOT time
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
        Debug.Log("EndDialogue called");
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        if (continueText != null) continueText.gameObject.SetActive(false);
        StartCoroutine(TransitionSequence());
    }
    
    IEnumerator TransitionSequence()
    {
        if (blackScreenPanel == null) yield break;
        
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
        
        // WAIT 5 SECONDS
        yield return new WaitForSeconds(5f);
        
        // REMOVE NUN
        NPCInteraction npc = FindObjectOfType<NPCInteraction>();
        if (npc != null) npc.gameObject.SetActive(false);
        
        // RESTORE CONTROLS
        if (playerController != null) playerController.enabled = true;
        if (playerFollowCamera != null) playerFollowCamera.enabled = true;
        
        // RESTORE CAMERA
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPos;
            mainCamera.transform.rotation = originalCameraRot;
        }
        
        // SHOW PLAYER CHARACTER AGAIN
        if (playerModel != null) playerModel.SetActive(true);
        
        // NOTIFY GAME PROGRESS MANAGER
        GameProgressManager progressManager = FindObjectOfType<GameProgressManager>();
        if (progressManager != null)
        {
            progressManager.CompleteNunDialogue();
        }
        
        // SHOW MAP PROMPT
        ShowMapPrompt();
        
        // MARK THAT PLAYER HAS RECEIVED THE MAP
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.MapReceived();
            Debug.Log("Map received notification sent to GameProgressManager");
        }
        
        // FADE BACK IN
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        cg.alpha = 0f;
    }
    
    void ShowMapPrompt()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
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

