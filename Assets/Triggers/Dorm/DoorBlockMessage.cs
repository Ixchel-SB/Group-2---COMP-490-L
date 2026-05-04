using UnityEngine;
using TMPro;
using System.Collections;

public class DoorBlockMessage : MonoBehaviour
{
    public TextMeshProUGUI thinkingText;
    public float displayDuration = 2f;
    
    private CanvasGroup canvasGroup;
    private bool isShowing = false;
    private static DoorBlockMessage instance;
    
    void Awake()
    {
        // Make sure the GameObject is active
        if (!gameObject.activeSelf)
        {
            Debug.LogWarning("DoorBlockMessage GameObject was inactive - please activate it in the Hierarchy");
        }
        
        instance = this;
    }
    
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
        
        // If this GameObject is inactive, create a temporary one
        if (!gameObject.activeSelf)
        {
            Debug.Log("DoorBlockMessage GameObject is inactive - creating temporary message");
            CreateTemporaryMessage(message);
            return;
        }
        
        StartCoroutine(DisplayMessage(message));
    }
    
    void CreateTemporaryMessage(string message)
    {
        // Create a temporary canvas for the message
        GameObject tempCanvas = new GameObject("TempMessageCanvas");
        Canvas canvas = tempCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        // Add a panel for background
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(tempCanvas.transform);
        UnityEngine.UI.Image panelImage = panel.AddComponent<UnityEngine.UI.Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);
        
        // Add text
        GameObject textObj = new GameObject("MessageText");
        textObj.transform.SetParent(panel.transform);
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = message;
        tmpText.fontSize = 36;
        tmpText.color = Color.white;
        tmpText.alignment = TextAlignmentOptions.Center;
        
        // Position the panel
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 0);
        panelRect.anchorMax = new Vector2(1, 1);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Position the text
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Destroy after duration
        Destroy(tempCanvas, displayDuration);
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
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.3f);
            yield return null;
        }
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
        
        // Wait
        yield return new WaitForSeconds(displayDuration);
        
        // Fade out
        elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.3f);
            yield return null;
        }
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
        thinkingText.gameObject.SetActive(false);
        
        isShowing = false;
    }
}
