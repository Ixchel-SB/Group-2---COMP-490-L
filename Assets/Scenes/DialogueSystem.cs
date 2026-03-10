using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueSystem : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        [TextArea(2, 4)]
        public string line;
    }
    
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI continueText; // "Press F to continue"
    
    public List<DialogueLine> dialogueLines;
    public float typingSpeed = 0.05f;
    
    private int currentLine = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    
    void Start()
    {
        dialoguePanel.SetActive(false);
        if (continueText != null)
            continueText.text = "Press F to continue";
    }
    
    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.F) && !isTyping)
        {
            NextLine();
        }
    }
    
    public void StartDialogue()
    {
        currentLine = 0;
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        
        // Freeze player movement (optional)
        // FindObjectOfType<PlayerController>().enabled = false;
        
        DisplayLine();
    }
    
    void DisplayLine()
    {
        if (currentLine < dialogueLines.Count)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            
            speakerText.text = dialogueLines[currentLine].speakerName + ":";
            typingCoroutine = StartCoroutine(TypeLine(dialogueLines[currentLine].line));
        }
        else
        {
            EndDialogue();
        }
    }
    
    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
    }
    
    void NextLine()
    {
        currentLine++;
        DisplayLine();
    }
    
    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        
        // Unfreeze player movement
        // FindObjectOfType<PlayerController>().enabled = true;
        
        // Optional: Trigger map give event
        Debug.Log("Dialogue ended - map should be given here");
    }
}