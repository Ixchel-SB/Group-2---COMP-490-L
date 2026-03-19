using UnityEngine;

public class OptionsTabs : MonoBehaviour
{
    public GameObject audioPanel;
    public GameObject accessibilityPanel;
    
    void Start()
    {
        // Start with audio panel visible
        ShowAudio();
    }
    
    public void ShowAudio()
    {
        audioPanel.SetActive(true);
        accessibilityPanel.SetActive(false);
    }
    
    public void ShowAccessibility()
    {
        audioPanel.SetActive(false);
        accessibilityPanel.SetActive(true);
    }
}