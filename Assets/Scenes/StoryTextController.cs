using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class StoryTextController : MonoBehaviour
{
    [System.Serializable]
    public class TextSegment
    {
        [TextArea(1, 3)]
        public string text;
        public float typingSpeed = 0.05f;
        public float holdTime = 1.5f;
    }

    public TextMeshProUGUI storyText;
    public TextSegment[] textSegments;
    public string nextSceneName = "Scenes/TrainCutscene";
    
    void Start()
    {
        Debug.Log("StoryTextController Start - looking for text");
        
        if (storyText == null)
        {
            Debug.LogError("STORY TEXT IS NOT ASSIGNED! Drag your TextMeshPro element into the field.");
            return;
        }
        
        Debug.Log("Story text found: '" + storyText.text + "'");
        Debug.Log("Number of segments: " + textSegments.Length);
        
        if (textSegments.Length == 0)
        {
            Debug.LogError("No text segments defined! Add segments in the Inspector.");
            return;
        }
        
        StartCoroutine(PlayStory());
    }
    
    IEnumerator PlayStory()
    {
        for (int i = 0; i < textSegments.Length; i++)
        {
            Debug.Log("Playing segment " + i + ": " + textSegments[i].text);
            Debug.Log("Typing speed: " + textSegments[i].typingSpeed + ", Hold time: " + textSegments[i].holdTime);
            
            yield return StartCoroutine(TypeText(textSegments[i].text, textSegments[i].typingSpeed));
            
            Debug.Log("Segment " + i + " finished typing, holding for " + textSegments[i].holdTime + " seconds");
            yield return new WaitForSeconds(textSegments[i].holdTime);
        }
        
        Debug.Log("All segments complete - loading " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
    
    IEnumerator TypeText(string text, float speed)
    {
        storyText.text = "";
        
        foreach (char c in text.ToCharArray())
        {
            storyText.text += c;
            yield return new WaitForSeconds(speed);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Story skipped by player");
            StopAllCoroutines();
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
