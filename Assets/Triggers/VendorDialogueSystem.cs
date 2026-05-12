using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class VendorDialogueSystem : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        [TextArea(2, 4)]
        public string line;
        public bool hasChoices = false;
        public string[] choiceTexts = new string[4];
    }
    
    //UI Elements
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI continueText;
    
    //Choice UI Elements
    public GameObject choicesPanel;
    public Button[] choiceButtons; //Array of 4 buttons
    public TextMeshProUGUI[] choiceButtonTexts; // Text for each button
    
    public List<DialogueLine> dialogueLines;
    public float typingSpeed = 0.05f;
    
    [Header("Camera Settings")]
    public Camera mainCamera;
    public Transform dialogueCameraPosition;
    public MonoBehaviour playerFollowCamera;
    
    [Header("After Dialogue")]
    public GameObject objectToActivate; //Item to give to player
    public string nextSceneName = "";
    public string itemGivenMessage = "You received a map!";
    
    private int currentLine = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private bool waitingForChoice = false;
    private Coroutine typingCoroutine;
    private string selectedFood = "";
    
    private GameObject player;
    private MonoBehaviour playerController;
    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;
    
    void Start()
    {
        dialoguePanel.SetActive(false);
        if (continueText != null)
            continueText.text = "Press F to continue";
        
        //Hide choices panel initially
        if (choicesPanel != null)
            choicesPanel.SetActive(false);
        
        //Setup choice buttons
        if (choiceButtons != null)
        {
            Debug.Log("Setting up " + choiceButtons.Length + " choice buttons");
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                int index = i; // Capture for lambda
                if (choiceButtons[i] != null)
                {
                    //Remove any existing listeners to avoid duplicates
                    choiceButtons[i].onClick.RemoveAllListeners();
                    choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(index));
                    Debug.Log("Button " + i + " listener added: " + choiceButtons[i].name);
                }
                else
                {
                    Debug.LogError("Button " + i + " is null! Make sure all buttons are assigned in the Inspector.");
                }
            }
        }
        else
        {
            Debug.LogError("choiceButtons array is null! Assign buttons in Inspector.");
        }
        
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            playerController = player.GetComponent<MonoBehaviour>();
            if (playerController == null)
            {
                Transform playerArmature = player.transform.Find("PlayerArmature");
                if (playerArmature != null)
                    playerController = playerArmature.GetComponent<MonoBehaviour>();
            }
        }
    }
    
    void Update()
    {
        if (isDialogueActive && !waitingForChoice && Input.GetKeyDown(KeyCode.F) && !isTyping)
        {
            NextLine();
        }
        
        //NUMBER KEYS FOR CHOICES (1,2,3,4)
        if (waitingForChoice)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) OnChoiceSelected(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) OnChoiceSelected(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) OnChoiceSelected(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4)) OnChoiceSelected(3);
        }
    }
    
    public void StartDialogue()
    {
        currentLine = 0;
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        
        //UNLOCK THE CURSOR SO PLAYER CAN CLICK BUTTONS
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (choicesPanel != null)
            choicesPanel.SetActive(false);
        
        if (playerController != null)
            playerController.enabled = false;
        if (playerFollowCamera != null)
            playerFollowCamera.enabled = false;
        
        if (mainCamera != null && dialogueCameraPosition != null)
        {
            originalCameraPos = mainCamera.transform.position;
            originalCameraRot = mainCamera.transform.rotation;
            mainCamera.transform.position = dialogueCameraPosition.position;
            mainCamera.transform.rotation = dialogueCameraPosition.rotation;
            Debug.Log("Camera moved to vendor position");
        }
        
        DisplayLine();
    }
    
    void DisplayLine()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        
        //Hide choices panel when showing regular dialogue
        if (choicesPanel != null)
            choicesPanel.SetActive(false);
        
        waitingForChoice = false;
        
        DialogueLine line = dialogueLines[currentLine];
        speakerText.text = line.speakerName + ":";
        
        if (line.hasChoices)
        {
            //This line has choices - type it, then show choices
            typingCoroutine = StartCoroutine(TypeLineWithChoices(line.line, line.choiceTexts));
        }
        else
        {
            typingCoroutine = StartCoroutine(TypeLine(line.line));
        }
    }
    
    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        if (continueText != null) continueText.gameObject.SetActive(false);
        
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
        if (continueText != null) continueText.gameObject.SetActive(true);
    }
    
    IEnumerator TypeLineWithChoices(string line, string[] choices)
    {
        isTyping = true;
        dialogueText.text = "";
        if (continueText != null) continueText.gameObject.SetActive(false);
        
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
        
        // Show choices after typing is complete
        waitingForChoice = true;
        if (choicesPanel != null)
        {
            choicesPanel.SetActive(true);
            Debug.Log("ChoicesPanel activated");
            
            //Force cursor visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            //Force enable all buttons and log their state
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (choiceButtons[i] != null)
                {
                    choiceButtons[i].interactable = true;
                    Debug.Log("Button " + i + " - Name: " + choiceButtons[i].name);
                    Debug.Log("Button " + i + " - Interactable: " + choiceButtons[i].interactable);
                    
                    //Check if Image component has Raycast Target
                    Image btnImage = choiceButtons[i].GetComponent<Image>();
                    if (btnImage != null)
                    {
                        Debug.Log("Button " + i + " - Raycast Target: " + btnImage.raycastTarget);
                    }
                }
                else
                {
                    Debug.LogError("Button " + i + " is NULL in the array!");
                }
            }
        }
        else
        {
            Debug.LogError("ChoicesPanel is null!");
        }
    }
    
    void OnChoiceSelected(int choiceIndex)
    {
        Debug.Log("!!! BUTTON CLICK DETECTED !!! Index: " + choiceIndex);
        
        if (!waitingForChoice) 
        {
            Debug.Log("Not waiting for choice - ignoring");
            return;
        }
        
        Debug.Log("Choice accepted! Processing selection...");
        waitingForChoice = false;
        
        //Hide choices
        if (choicesPanel != null)
            choicesPanel.SetActive(false);
        
        //Set selected food based on choice
        switch (choiceIndex)
        {
            case 0:
                selectedFood = "Sub Sandwich";
                break;
            case 1:
                selectedFood = "Chocolate Brioche";
                break;
            case 2:
                selectedFood = "Spice Cupcake";
                break;
            case 3:
                selectedFood = "Frosted Cake";
                break;
        }
        
        Debug.Log("Player selected: " + selectedFood);
        
        //SAVE SELECTED FOOD TO PLAYERPREFS IMMEDIATELY!!
        PlayerPrefs.SetString("SelectedFood", selectedFood);
        PlayerPrefs.Save();
        Debug.Log("Food saved to PlayerPrefs: " + selectedFood);
        
        //Move to next line (the response after choice)
        currentLine++;
        
        //Insert the selected food into the response line
        if (currentLine < dialogueLines.Count)
        {
            string responseLine = dialogueLines[currentLine].line;
            responseLine = responseLine.Replace("{food}", selectedFood);
            dialogueLines[currentLine].line = responseLine;
            Debug.Log("Response line updated to: " + responseLine);
        }
        
        DisplayLine();
    }
    
    void NextLine()
    {
        currentLine++;
        
        if (currentLine < dialogueLines.Count)
        {
            DisplayLine();
        }
        else
        {
            EndDialogue();
        }
    }
    
    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        if (choicesPanel != null)
            choicesPanel.SetActive(false);
        if (continueText != null) continueText.gameObject.SetActive(false);
        
        if (playerController != null)
            playerController.enabled = true;
        if (playerFollowCamera != null)
            playerFollowCamera.enabled = true;
        
        //LOCK THE CURSOR AGAIN WHEN DIALOGUE ENDS!!~
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPos;
            mainCamera.transform.rotation = originalCameraRot;
            Debug.Log("Camera restored to player");
        }
        
        //NOTIFY GAME PROGRESS MANAGER THAT VENDOR DIALOGUE IS COMPLETE
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.CompleteVendorDialogue(selectedFood);
            Debug.Log("Vendor dialogue completed - notified GameProgressManager");
        }
        
        //Show what the player chose
        string message = "You chose the " + selectedFood + "! Enjoy!";
        StartCoroutine(ShowMessage(message));
        
        //Add to inventory
        AddToInventory(selectedFood);
        
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
        
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            StartCoroutine(LoadNextScene());
        }
        
        Debug.Log("Vendor dialogue ended - Player chose: " + selectedFood);
    }
    
    void AddToInventory(string food)
    {
        Debug.Log("Adding to inventory: " + food);
        //TODO: Add your inventory system logic here
        //Example: InventoryManager.Instance.AddItem(food);
        
        //Show confirmation in console
        Debug.Log("Inventory updated with: " + food);
    }
    
    IEnumerator ShowMessage(string message)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) yield break;
        
        GameObject msgObj = new GameObject("TempMessage");
        msgObj.transform.SetParent(canvas.transform, false);
        
        TextMeshProUGUI tmp = msgObj.AddComponent<TextMeshProUGUI>();
        tmp.text = message;
        tmp.fontSize = 28;
        tmp.color = Color.yellow;
        tmp.alignment = TextAlignmentOptions.Center;
        
        RectTransform rect = msgObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0, 200);
        rect.sizeDelta = new Vector2(600, 60);
        
        CanvasGroup cg = msgObj.AddComponent<CanvasGroup>();
        
        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
            yield return null;
        }
        
        yield return new WaitForSeconds(2f);
        
        elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
            yield return null;
        }
        
        Destroy(msgObj);
    }
    
    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }
}
