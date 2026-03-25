using UnityEngine;
using TMPro; // Add this for debugging

public class OptionsTabs : MonoBehaviour
{
    public GameObject audioPanel;
    public GameObject accessibilityPanel;
    
    void Start()
    {
        Debug.Log("OptionsTabs started");
        
        // Make sure panels start correctly
        if (audioPanel != null)
        {
            audioPanel.SetActive(true);
            Debug.Log("Audio panel activated");
        }
        else
        {
            Debug.LogError("Audio Panel not assigned in OptionsTabs!");
        }
        
        if (accessibilityPanel != null)
        {
            accessibilityPanel.SetActive(false);
            Debug.Log("Accessibility panel deactivated");
        }
        else
        {
            Debug.LogError("Accessibility Panel not assigned in OptionsTabs!");
        }
        
        // Debug: Check all text in this canvas
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);
        Debug.Log("OptionsTabs found " + texts.Length + " text elements");
    }
    
    public void ShowAudio()
    {
        Debug.Log("ShowAudio called");
        if (audioPanel != null)
        {
            audioPanel.SetActive(true);
            Debug.Log("Audio panel shown");
        }
        
        if (accessibilityPanel != null)
        {
            accessibilityPanel.SetActive(false);
            Debug.Log("Accessibility panel hidden");
        }
    }
    
    public void ShowAccessibility()
    {
        Debug.Log("ShowAccessibility called");
        if (audioPanel != null)
        {
            audioPanel.SetActive(false);
            Debug.Log("Audio panel hidden");
        }
        
        if (accessibilityPanel != null)
        {
            accessibilityPanel.SetActive(true);
            Debug.Log("Accessibility panel shown");
        }
    }
}