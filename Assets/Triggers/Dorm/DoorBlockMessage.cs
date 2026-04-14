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
        if (thinkingText != null)
        {
            canvasGroup = thinkingText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = thinkingText.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            thinkingText.gameObject.SetActive(false);
        }
    }
    
    public void ShowMessage(string message)
    {
        StartCoroutine(DisplayMessage(message));
    }
    
    IEnumerator DisplayMessage(string message)
    {
        if (thinkingText == null) yield break;
        
        thinkingText.gameObject.SetActive(true);
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
        thinkingText.gameObject.SetActive(false);
    }
}
