using UnityEngine;
using TMPro;
using System.Collections;

public class PlazaTriggerText : MonoBehaviour
{
    [Header("Text Settings")]
    public TextMeshProUGUI thinkingText;
    public string message = "I should get something to eat... I smell something good";
    public float typingSpeed = 0.05f;
    
    [Header("Reveal Object")]
    public GameObject arrowObject;
    
    private bool hasTriggered = false;
    private CanvasGroup canvasGroup;
    private bool waitingForF = false;
    private bool canActivate = false;
    
    void Start()
    {
        Debug.Log("PlazaTriggerText Start called");
        
        // Setup thinking text
        if (thinkingText != null)
        {
            canvasGroup = thinkingText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = thinkingText.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            thinkingText.gameObject.SetActive(false);
            Debug.Log("ThinkingText assigned and hidden");
        }
        else
        {
            Debug.LogError("ThinkingText is NOT assigned in PlazaTriggerText!");
        }
        
        // Hide arrow at start
        if (arrowObject != null)
        {
            arrowObject.SetActive(false);
            Debug.Log("ArrowObject hidden at start");
        }
        else
        {
            Debug.LogError("ArrowObject is NOT assigned in PlazaTriggerText!");
        }
        
        // Check if Nun dialogue is completed
        if (GameProgressManager.Instance != null)
        {
            canActivate = GameProgressManager.Instance.IsNunDialogueCompleted();
            Debug.Log("Nun dialogue completed: " + canActivate);
        }
        else
        {
            Debug.LogWarning("GameProgressManager.Instance is NULL");
            canActivate = false;
        }
        
        // Enable/disable trigger
        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = canActivate;
        
        Debug.Log("PlazaTrigger ready - canActivate: " + canActivate);
    }
    
    void Update()
    {
        if (waitingForF && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F key pressed - current arrow state: " + (arrowObject != null ? arrowObject.activeSelf.ToString() : "NULL"));
            StartCoroutine(HideTextAndRevealArrow());
        }
        
        // Debug: Check arrow state every frame while waiting
        if (waitingForF && arrowObject != null)
        {
            if (!arrowObject.activeSelf)
            {
                Debug.Log("Arrow is STILL disabled while waiting for F");
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered && canActivate)
        {
            Debug.Log("Player entered plaza trigger - showing thinking text");
            StartCoroutine(ShowThinkingText());
        }
        else if (other.CompareTag("Player") && !canActivate)
        {
            Debug.Log("Player entered plaza trigger but not activated yet (Nun dialogue not complete)");
        }
    }
    
    public void EnableTrigger()
    {
        canActivate = true;
        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = true;
        Debug.Log("PlazaTrigger enabled!");
    }
    
    IEnumerator ShowThinkingText()
    {
        Debug.Log("ShowThinkingText started");
        if (thinkingText == null) yield break;
        
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = "";
        
        // Fade in
        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        Debug.Log("ThinkingText faded in");
        
        // Type out text
        foreach (char c in message.ToCharArray())
        {
            thinkingText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        Debug.Log("ThinkingText fully typed - waiting for F key");
        waitingForF = true;
    }
    
    IEnumerator HideTextAndRevealArrow()
    {
        Debug.Log("=== HIDE TEXT AND REVEAL ARROW STARTED ===");
        waitingForF = false;
        
        // Fade out
        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        thinkingText.gameObject.SetActive(false);
        Debug.Log("Text hidden");
        
        // REVEAL ARROW - FORCE ENABLE EVERYTHING
        if (arrowObject != null)
        {
            Debug.Log("ArrowObject found: " + arrowObject.name);
            
            // Force enable the arrow object
            arrowObject.SetActive(true);
            Debug.Log("ArrowObject active: " + arrowObject.activeSelf);
            
            // Force enable ALL children
            foreach (Transform child in arrowObject.transform)
            {
                child.gameObject.SetActive(true);
                Debug.Log("Enabled child: " + child.name);
            }
            
            // Force enable renderers
            Renderer[] renderers = arrowObject.GetComponentsInChildren<Renderer>(true);
            Debug.Log("Found " + renderers.Length + " renderers");
            foreach (Renderer r in renderers)
            {
                r.enabled = true;
                Debug.Log("Enabled renderer on: " + r.gameObject.name);
            }
            
            // Log the position
            Debug.Log("Arrow position: " + arrowObject.transform.position);
            
            // Also try moving it slightly to see if it's just hidden
            arrowObject.transform.position = new Vector3(
                arrowObject.transform.position.x,
                arrowObject.transform.position.y + 1f,
                arrowObject.transform.position.z
            );
            Debug.Log("Moved arrow up by 1 unit to make it visible");
        }
        else
        {
            Debug.LogError("ArrowObject is NULL!");
        }
        
        hasTriggered = true;
        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = false;
        
        Debug.Log("=== HIDE TEXT AND REVEAL ARROW COMPLETED ===");
    }
}
