using UnityEngine;
using TMPro;
using System.Collections;

public class ThinkingText : MonoBehaviour
{
    public TextMeshProUGUI thinkingText;
    public string message = "I need to find Sister Mary and talk to her. - Meztly is thinking";
    public float displayDuration = 5f;
    public float fadeSpeed = 2f;
    
    private CanvasGroup canvasGroup;
    
    void Start()
    {
        canvasGroup = thinkingText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = thinkingText.gameObject.AddComponent<CanvasGroup>();
        
        StartCoroutine(ShowThinkingText());
    }
    
    IEnumerator ShowThinkingText()
    {
        thinkingText.text = message;
        
        // Fade in
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            canvasGroup.alpha = alpha;
            yield return null;
        }
        
        // Hold
        yield return new WaitForSeconds(displayDuration);
        
        // Fade out
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            canvasGroup.alpha = alpha;
            yield return null;
        }
        
        thinkingText.gameObject.SetActive(false);
    }
}