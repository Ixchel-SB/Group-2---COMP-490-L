using UnityEngine;
using TMPro;
using System.Collections;

public class CemeteryInteraction : MonoBehaviour
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
    
    [Header("Character Models - OLD (to remove)")]
    public GameObject oldMarcelo;     // Marcelo_Sitting (dining room version)
    public GameObject oldElio;        // Elio_Sitting (dining room version)
    public GameObject oldValentina;   // Valentina_Sitting (dining room version)
    
    [Header("Character Models - NEW (to enable at cemetery)")]
    public GameObject newMarcelo;     // Marcelo_Male Standing Pose
    public GameObject newElio;        // Elio_Head Nod
    public GameObject newValentina;   // Valentina_Sitting Idle
    
    [Header("Thinking Text")]
    public string cemeteryThinkingText = "I should go to the cemetery... everyone is probably waiting for me.";
    public float cemeteryThinkingDuration = 3f;
    
    [Header("Dialogue Lines")]
    [TextArea(3, 5)]
    public string[] marceloDialogueLines = new string[]
    {
        "About time you showed up!",
        "We were starting to think you got lost."
    };
    
    [TextArea(3, 5)]
    public string[] elioDialogueLines = new string[]
    {
        "Yeah, we've been waiting forever.",
        "The cemetery is this way."
    };
    
    [TextArea(3, 5)]
    public string[] valentinaDialogueLines = new string[]
    {
        "Don't be too hard on her. She had a lot to do.",
        "Let's go before it gets dark."
    };
    
    [TextArea(3, 5)]
    public string[] metzlyDialogueLines = new string[]
    {
        "Sorry I'm late... I had to take care of a few things.",
        "I'm ready now."
    };
    
    [Header("Final Thinking Text")]
    public string finalThinkingText = "The cemetery is quiet... too quiet. But at least everyone is here.";
    public float finalThinkingDuration = 4f;
    
    private CanvasGroup thinkingCanvasGroup;
    private CanvasGroup blackCanvasGroup;
    private bool hasInteracted = false;
    private bool isSequenceRunning = false;
    private bool waitingForF = false;
    private bool playerInRange = false;
    private bool modelsSwapped = false;
    
    private GameObject player;
    private MonoBehaviour playerController;
    private GameObject promptObject;
    private TextMeshProUGUI promptText;
    
    void Start()
    {
        Debug.Log("=== CEMETERY INTERACTION START ===");
        
        // Find player
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
        
        // Setup thinking text
        if (thinkingText != null)
        {
            thinkingCanvasGroup = thinkingText.GetComponent<CanvasGroup>();
            if (thinkingCanvasGroup == null)
                thinkingCanvasGroup = thinkingText.gameObject.AddComponent<CanvasGroup>();
            thinkingCanvasGroup.alpha = 0f;
            thinkingText.gameObject.SetActive(false);
        }
        
        // Setup dialogue UI
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (continueText != null)
            continueText.text = "Press F to continue";
        
        // Setup black screen
        if (blackScreenPanel != null)
        {
            blackCanvasGroup = blackScreenPanel.GetComponent<CanvasGroup>();
            if (blackCanvasGroup == null)
                blackCanvasGroup = blackScreenPanel.AddComponent<CanvasGroup>();
            blackCanvasGroup.alpha = 0f;
        }
        
        // Create interaction prompt above the character
        CreateInteractionPrompt();
        
        // Initially new models are disabled
        if (newMarcelo != null) newMarcelo.SetActive(false);
        if (newElio != null) newElio.SetActive(false);
        if (newValentina != null) newValentina.SetActive(false);
        
        Debug.Log("=== CEMETERY INTERACTION READY ===");
    }
    
    void CreateInteractionPrompt()
    {
        GameObject prompt = new GameObject("InteractionPrompt");
        prompt.transform.SetParent(transform);
        prompt.transform.localPosition = new Vector3(0, 2f, 0);
        
        TextMeshProUGUI text = prompt.AddComponent<TextMeshProUGUI>();
        text.text = "Press F to talk";
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        
        // Add a background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(prompt.transform);
        UnityEngine.UI.Image image = background.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(0, 0, 0, 0.7f);
        
        promptObject = prompt;
        promptText = text;
        prompt.SetActive(false);
    }
    
    void Update()
    {
        if (waitingForF && Input.GetKeyDown(KeyCode.F))
        {
            waitingForF = false;
        }
        
        // Check distance to player
        if (player != null && !hasInteracted && !isSequenceRunning)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            
            if (distance < interactionRange)
            {
                if (promptObject != null && !promptObject.activeSelf)
                    promptObject.SetActive(true);
                
                if (Input.GetKeyDown(interactKey))
                {
                    StartCoroutine(RunCemeterySequence());
                }
            }
            else
            {
                if (promptObject != null && promptObject.activeSelf)
                    promptObject.SetActive(false);
            }
        }
    }
    
    void FreezePlayer()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    void UnfreezePlayer()
    {
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
    
    IEnumerator ShowDialogue(string line, string speaker)
    {
        if (dialoguePanel == null)
        {
            Debug.LogError("DialoguePanel is NULL!");
            yield break;
        }
        
        dialoguePanel.SetActive(true);
        speakerText.text = speaker + ":";
        dialogueText.text = "";
        
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        
        if (continueText != null)
        {
            continueText.gameObject.SetActive(true);
        }
        
        waitingForF = true;
        yield return new WaitUntil(() => !waitingForF);
        
        if (continueText != null)
        {
            continueText.gameObject.SetActive(false);
        }
        
        dialoguePanel.SetActive(false);
    }
    
    IEnumerator RunCemeterySequence()
    {
        Debug.Log("=== CEMETERY SEQUENCE STARTED ===");
        
        hasInteracted = true;
        isSequenceRunning = true;
        
        // Hide prompt
        if (promptObject != null)
            promptObject.SetActive(false);
        
        FreezePlayer();
        
        // Show cemetery thinking text
        yield return StartCoroutine(ShowThinkingText(cemeteryThinkingText, cemeteryThinkingDuration));
        
        // Fade to black for model swap
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
            Debug.Log("Faded to black for model swap");
        }
        
        // Swap character models
        Debug.Log("Swapping to cemetery character models...");
        
        if (oldMarcelo != null) oldMarcelo.SetActive(false);
        if (oldElio != null) oldElio.SetActive(false);
        if (oldValentina != null) oldValentina.SetActive(false);
        
        if (newMarcelo != null) newMarcelo.SetActive(true);
        if (newElio != null) newElio.SetActive(true);
        if (newValentina != null) newValentina.SetActive(true);
        
        modelsSwapped = true;
        
        // Fade back
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
            Debug.Log("Faded back");
        }
        
        // Dialogue sequence
        for (int i = 0; i < marceloDialogueLines.Length; i++)
        {
            yield return StartCoroutine(ShowDialogue(marceloDialogueLines[i], "Marcelo"));
        }
        
        for (int i = 0; i < elioDialogueLines.Length; i++)
        {
            yield return StartCoroutine(ShowDialogue(elioDialogueLines[i], "Elio"));
        }
        
        for (int i = 0; i < valentinaDialogueLines.Length; i++)
        {
            yield return StartCoroutine(ShowDialogue(valentinaDialogueLines[i], "Valentina"));
        }
        
        for (int i = 0; i < metzlyDialogueLines.Length; i++)
        {
            yield return StartCoroutine(ShowDialogue(metzlyDialogueLines[i], "Metzly"));
        }
        
        // Show final thinking text
        yield return StartCoroutine(ShowThinkingText(finalThinkingText, finalThinkingDuration));
        
        UnfreezePlayer();
        isSequenceRunning = false;
        
        // Disable the script so player can't interact again
        this.enabled = false;
        
        Debug.Log("=== CEMETERY SEQUENCE COMPLETED ===");
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
