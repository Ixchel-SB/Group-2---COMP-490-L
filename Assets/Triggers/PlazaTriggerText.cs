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
        // Setup thinking text
        if (thinkingText != null)
        {
            canvasGroup = thinkingText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = thinkingText.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            thinkingText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("ThinkingText is NOT assigned in PlazaTriggerText!");
        }
        
        // Hide arrow at start
        if (arrowObject != null)
        {
            arrowObject.SetActive(false);
        }
        else
        {
            Debug.LogError("ArrowObject is NOT assigned in PlazaTriggerText!");
        }
        
        // Check if Nun dialogue is completed
        if (GameProgressManager.Instance != null)
        {
            canActivate = GameProgressManager.Instance.IsNunDialogueCompleted();
        }
        
        // Enable/disable trigger
        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = canActivate;
    }
    
    void Update()
    {
        if (waitingForF && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(HideTextAndRevealArrow());
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered && canActivate)
        {
            StartCoroutine(ShowThinkingText());
        }
    }
    
    public void EnableTrigger()
    {
        canActivate = true;
        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = true;
    }
    
    IEnumerator ShowThinkingText()
    {
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
        
        // Type out text
        foreach (char c in message.ToCharArray())
        {
            thinkingText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        waitingForF = true;
    }
    
    IEnumerator HideTextAndRevealArrow()
    {
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
        
        // Reveal arrow
        if (arrowObject != null)
        {
            arrowObject.SetActive(true);
        }
        
        hasTriggered = true;
        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = false;
    }
}
