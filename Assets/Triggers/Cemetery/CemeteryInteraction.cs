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
    
    [Header("Character Models - DINING ROOM VERSIONS (to remove)")]
    public GameObject diningRoomMarcelo;     // Marcelo_Sitting (dining room)
    public GameObject diningRoomElio;        // Elio_Sitting (dining room)
    public GameObject diningRoomValentina;   // Valentina_Sitting (dining room)
    
    [Header("Character Models - CEMETERY INITIAL VERSIONS (to enable AFTER Samael dialogue)")]
    public GameObject cemeteryInitialMarcelo;    // Marcelo_Male Standing Pose
    public GameObject cemeteryInitialElio;       // Elio_Head Nod
    public GameObject cemeteryInitialValentina;  // Valentina_Sitting Idle (THIS script should be on this object)
    
    [Header("Character Models - CEMETERY FINAL VERSIONS (to enable AFTER dialogue)")]
    public GameObject cemeteryFinalMarcelo;      // Marcelo_Standing W_Briefcase Idle
    public GameObject cemeteryFinalValentina;    // Valentina_Sitting Talking
    // Elio stays the same (Elio_Head Nod)
    
    [Header("Thinking Text")]
    public string cemeteryThinkingText = "I should go to the cemetery... everyone is probably waiting for me.";
    public float cemeteryThinkingDuration = 3f;
    
    [Header("Dialogue Lines (in order)")]
    [TextArea(3, 5)]
    public string[] dialogueLines = new string[]
    {
        "Sorry I'm late... I had a lot to do.",  // Metzly
        "About time! We've been waiting forever.",  // Elio
        "Yeah, don't keep us waiting like that again.",  // Marcelo
        "Let's not argue. She's here now and that's what matters.",  // Valentina
        "You're right. Let's head to the cemetery.",  // Marcelo & Metzly together
        "It's getting dark soon. We should hurry.",  // Elio
        "The cemetery is just ahead.",  // Valentina
        "Wait... do you hear that?",  // Elio
        "Hear what? I don't hear anything.",  // Metzly
        "It's probably just the wind. Let's keep moving.",  // Marcelo
        "I hope you're right...",  // Elio
        "Come on, everyone. Stay close together."  // Valentina
    };
    
    [Header("Speakers for each dialogue line")]
    public string[] speakers = new string[]
    {
        "Metzly",
        "Elio",
        "Marcelo",
        "Valentina",
        "Marcelo & Metzly",
        "Elio",
        "Valentina",
        "Elio",
        "Metzly",
        "Marcelo",
        "Elio",
        "Valentina"
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
    private bool isEnabledBySamael = false;  // NEW: Track if Samael dialogue has enabled this
    
    private GameObject player;
    private MonoBehaviour playerController;
    private GameObject promptObject;
    private TextMeshProUGUI promptText;
    
    void Awake()
    {
        Debug.Log("=== CEMETERY INTERACTION AWAKE on " + gameObject.name + " ===");
        
        //FORCE DISABLE all models at the VERY beginning
        //This GameObject (Valentina_Sitting Idle) should be disabled
        gameObject.SetActive(false);
        
        if (cemeteryInitialMarcelo != null)
        {
            cemeteryInitialMarcelo.SetActive(false);
            Debug.Log("FORCE DISABLED Initial Marcelo in Awake");
        }
        
        if (cemeteryInitialElio != null)
        {
            cemeteryInitialElio.SetActive(false);
            Debug.Log("FORCE DISABLED Initial Elio in Awake");
        }
        
        if (cemeteryFinalMarcelo != null)
        {
            cemeteryFinalMarcelo.SetActive(false);
            Debug.Log("FORCE DISABLED Final Marcelo in Awake");
        }
        
        if (cemeteryFinalValentina != null)
        {
            cemeteryFinalValentina.SetActive(false);
            Debug.Log("FORCE DISABLED Final Valentina in Awake");
        }
    }
    
    void Start()
    {
        Debug.Log("=== CEMETERY INTERACTION START on " + gameObject.name + " ===");
        
        //Find player (T____T)
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
        
        //Setup thinking text
        if (thinkingText != null)
        {
            thinkingCanvasGroup = thinkingText.GetComponent<CanvasGroup>();
            if (thinkingCanvasGroup == null)
                thinkingCanvasGroup = thinkingText.gameObject.AddComponent<CanvasGroup>();
            thinkingCanvasGroup.alpha = 0f;
            thinkingText.gameObject.SetActive(false);
        }
        
        //Setup dialogue UI
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (continueText != null)
            continueText.text = "Press F to continue";
        
        //Setup black screen
        if (blackScreenPanel != null)
        {
            blackCanvasGroup = blackScreenPanel.GetComponent<CanvasGroup>();
            if (blackCanvasGroup == null)
                blackCanvasGroup = blackScreenPanel.AddComponent<CanvasGroup>();
            blackCanvasGroup.alpha = 0f;
        }
        
        //Create interaction prompt above the character
        CreateInteractionPrompt();
        
        //Ensure everything is disabled at start
        gameObject.SetActive(false);
        if (cemeteryInitialMarcelo != null) cemeteryInitialMarcelo.SetActive(false);
        if (cemeteryInitialElio != null) cemeteryInitialElio.SetActive(false);
        if (cemeteryFinalMarcelo != null) cemeteryFinalMarcelo.SetActive(false);
        if (cemeteryFinalValentina != null) cemeteryFinalValentina.SetActive(false);
        
        Debug.Log("=== CEMETERY INTERACTION READY on " + gameObject.name + " - ALL MODELS DISABLED ===");
    }
    
    // Call this method from DiningRoomInteraction to enable initial cemetery models (ONLY after Samael dialogue)
    public void EnableInitialCemeteryModels()
    {
        Debug.Log("=== ENABLING INITIAL CEMETERY MODELS (Called from Samael dialogue) ===");
        
        isEnabledBySamael = true;
        
        //Enable this GameObject (Valentina_Sitting Idle)
        gameObject.SetActive(true);
        Debug.Log("Enabled " + gameObject.name);
        
        if (cemeteryInitialMarcelo != null)
        {
            cemeteryInitialMarcelo.SetActive(true);
            Debug.Log("Initial Marcelo enabled");
        }
        
        if (cemeteryInitialElio != null)
        {
            cemeteryInitialElio.SetActive(true);
            Debug.Log("Initial Elio enabled");
        }
        
        //Ensure final models remain disabled
        if (cemeteryFinalMarcelo != null)
        {
            cemeteryFinalMarcelo.SetActive(false);
        }
        
        if (cemeteryFinalValentina != null)
        {
            cemeteryFinalValentina.SetActive(false);
        }
    }
    
    void CreateInteractionPrompt()
    {
        promptObject = new GameObject("InteractionPrompt");
        promptObject.transform.SetParent(transform);
        promptObject.transform.localPosition = new Vector3(0, 2f, 0);
        
        TextMeshProUGUI text = promptObject.AddComponent<TextMeshProUGUI>();
        text.text = "Press F to talk";
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        
        //Add a background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(promptObject.transform);
        UnityEngine.UI.Image image = background.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(0, 0, 0, 0.7f);
        
        //Set background to cover the text
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        promptObject.SetActive(false);
    }
    
    void Update()
    {
        if (waitingForF && Input.GetKeyDown(KeyCode.F))
        {
            waitingForF = false;
        }
        
        //Check distance to player - only if this object is active AND enabled by Samael
        if (player != null && !hasInteracted && !isSequenceRunning && gameObject.activeSelf && isEnabledBySamael)
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
        
        //Hide prompt
        if (promptObject != null)
            promptObject.SetActive(false);
        
        FreezePlayer();
        
        //Show cemetery thinking text
        yield return StartCoroutine(ShowThinkingText(cemeteryThinkingText, cemeteryThinkingDuration));
        
        //Run all dialogue lines in order
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            yield return StartCoroutine(ShowDialogue(dialogueLines[i], speakers[i]));
        }
        
        //Fade to black for final model swap
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
            Debug.Log("Faded to black for final model swap");
        }
        
        //Remove INITIAL cemetery models
        Debug.Log("Removing INITIAL cemetery character models...");
        
        //Disable this GameObject (Valentina_Sitting Idle)
        gameObject.SetActive(false);
        
        if (cemeteryInitialMarcelo != null) cemeteryInitialMarcelo.SetActive(false);
        //Elio stays the same - do not disable
        
        //Enable FINAL cemetery models
        Debug.Log("Enabling FINAL cemetery character models...");
        
        if (cemeteryFinalMarcelo != null) cemeteryFinalMarcelo.SetActive(true);
        if (cemeteryFinalValentina != null) cemeteryFinalValentina.SetActive(true);
        
        //Fade back
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
        
        //Show final thinking text!
        yield return StartCoroutine(ShowThinkingText(finalThinkingText, finalThinkingDuration));
        
        UnfreezePlayer();
        isSequenceRunning = false;
        
        //Disable this script so player can't interact again
        this.enabled = false;
        
        Debug.Log("=== CEMETERY SEQUENCE COMPLETED ===");
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
