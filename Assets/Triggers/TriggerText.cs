using UnityEngine;
using TMPro;
using System.Collections;

public class TriggerText : MonoBehaviour
{
    [Header("Text Settings")]
    public TextMeshProUGUI thinkingText;
    public string message = "I should get something to eat... I smell something good";
    public float typingSpeed = 0.05f;
    
    [Header("Reveal Object")]
    public GameObject arrowObject; // The arrow to reveal after text
    
    private bool hasTriggered = false;
    private CanvasGroup canvasGroup;
    private bool textActive = false;
    private bool waitingForF = false;
    
    void Start()
    {
        if (thinkingText != null)
        {
            canvasGroup = thinkingText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = thinkingText.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            thinkingText.gameObject.SetActive(false);
        }
        
        if (arrowObject != null)
            arrowObject.SetActive(false);
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
        if (other.CompareTag("Player") && !hasTriggered)
        {
            StartCoroutine(ShowThinkingText());
        }
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
        
        // Wait for player to press F
        waitingForF = true;
    }
    
    IEnumerator HideTextAndRevealArrow()
    {
        waitingForF = false;
        
        // Fade out thinking text
        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        thinkingText.gameObject.SetActive(false);
        
        // Reveal the arrow
        if (arrowObject != null)
        {
            arrowObject.SetActive(true);
            Debug.Log("Arrow revealed at position: " + arrowObject.transform.position);
        }
        
        hasTriggered = true;
        
        // Disable trigger so it doesn't happen again
        GetComponent<Collider>().enabled = false;
    }
}
