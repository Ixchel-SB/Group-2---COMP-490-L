using UnityEngine;
using TMPro;
using System.Collections;

public class DoorBlockMessage : MonoBehaviour
{
    public TextMeshProUGUI thinkingText;
    public float typingSpeed = 0.05f;
    public float displayDuration = 3f;
    
    private CanvasGroup canvasGroup;
    
    void Start()
    {
        // Make sure the GameObject is active
        gameObject.SetActive(true);
        
        if (thinkingText != null)
        {
            canvasGroup = thinkingText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = thinkingText.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            thinkingText.gameObject.SetActive(true);
        }
    }
    
    public void ShowMessage(string message)
    {
        // Ensure the GameObject is active before starting coroutine
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        
        StartCoroutine(DisplayMessage(message));
    }
    
    IEnumerator DisplayMessage(string message)
    {
        if (thinkingText == null) 
        {
            Debug.LogError("DoorBlockMessage: thinkingText is not assigned!");
            yield break;
        }
        
        thinkingText.text = "";
        
        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        
        foreach (char c in message.ToCharArray())
        {
            thinkingText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        yield return new WaitForSeconds(displayDuration);
        
        elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
