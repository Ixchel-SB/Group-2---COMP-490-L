using UnityEngine;
using TMPro;
using System.Collections;

public class DoorBlockMessage : MonoBehaviour
{
    public TextMeshProUGUI thinkingText;
    public float displayDuration = 2f;
    
    private CanvasGroup canvasGroup;
    private bool isShowing = false;
    
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
        else
        {
            Debug.LogError("DoorBlockMessage: thinkingText is not assigned!");
        }
    }
    
    public void ShowMessage(string message)
    {
        if (isShowing) return;
        StartCoroutine(DisplayMessage(message));
    }
    
    IEnumerator DisplayMessage(string message)
    {
        isShowing = true;
        
        if (thinkingText == null) 
        {
            isShowing = false;
            yield break;
        }
        
        thinkingText.text = message;
        thinkingText.gameObject.SetActive(true);
        
        // Fade in
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.3f);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        
        // Wait
        yield return new WaitForSeconds(displayDuration);
        
        // Fade out
        elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.3f);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        thinkingText.gameObject.SetActive(false);
        
        isShowing = false;
    }
}
