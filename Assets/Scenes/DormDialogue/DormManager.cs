using UnityEngine;
using TMPro;
using System.Collections;

public class DormManager : MonoBehaviour
{
    private bool valentinaFirstTalked = false;
    private bool marceloTalked = false;
    private bool elioTalked = false;
    private bool foodEaten = false;
    private bool shovelPickedUp = false;
    private bool photoFound = false;
    private bool photoInspected = false;
    private bool timeAdvanced = false;
    private string selectedFood = "";
    private bool eatReminderShown = false;
    private bool isPhotoVisible = false;
    private Coroutine rotationCoroutine;
    
    public GameObject eatFoodPrompt;
    
    [Header("Food Items")]
    public GameObject foodSubSandwich;
    public GameObject foodChocolateBrioche;
    public GameObject foodSpiceCupcake;
    public GameObject foodFrostedCake;
    private GameObject currentFoodItem;
    
    [Header("Valentina Characters")]
    public GameObject valentinaHallway;
    public GameObject valentinaRoom;
    public Transform valentinaRoomPosition;
    
    [Header("Thinking Text")]
    public TextMeshProUGUI thinkingText;
    public string eatReminderMessage = "I should eat the food I brought before I forget";
    public string eatReminderDoorMessage = "I should eat my food before exploring more...";
    public string photoFoundMessage = "Who is that man with my sister... I wonder if Valentina knows something about this.";
    public float thinkingTextDuration = 5f;
    
    [Header("Backyard")]
    public GameObject graveInteraction;
    public GameObject shovelInteraction;
    public GameObject photoObject;
    public float photoDistanceFromCamera = 1.5f;
    public GameObject blackScreenPanel;
    
    [Header("Valentina Second Dialogue")]
    public RoommateDialogue valentinaSecondDialogue;
    
    private CanvasGroup thinkingCanvasGroup;
    private bool waitingForPhotoInspection = false;
    private ShovelInteraction shovelScript;
    private GameObject player;
    private MonoBehaviour playerController;
    private Camera mainCamera;
    private Vector3 originalPhotoPos;
    private Quaternion originalPhotoRot;
    private Vector3 originalPhotoScale;
    private CanvasGroup blackCanvasGroup;
    
    void Start()
    {
        Time.timeScale = 1f;
        
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = Camera.main;
        
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
        
        if (blackScreenPanel != null)
        {
            blackCanvasGroup = blackScreenPanel.GetComponent<CanvasGroup>();
            if (blackCanvasGroup == null)
                blackCanvasGroup = blackScreenPanel.AddComponent<CanvasGroup>();
            blackCanvasGroup.alpha = 0f;
        }
        
        selectedFood = PlayerPrefs.GetString("SelectedFood", "");
        Debug.Log("Player selected food: " + selectedFood);
        
        ShowCorrectFoodItem();
        
        if (eatFoodPrompt != null)
            eatFoodPrompt.SetActive(false);
        
        if (graveInteraction != null)
            graveInteraction.SetActive(true);
        
        if (shovelInteraction != null)
        {
            shovelInteraction.SetActive(true);
            shovelScript = shovelInteraction.GetComponent<ShovelInteraction>();
        }
        
        if (photoObject != null)
        {
            photoObject.SetActive(false);
            originalPhotoPos = photoObject.transform.position;
            originalPhotoRot = photoObject.transform.rotation;
            originalPhotoScale = photoObject.transform.localScale;
        }
        
        if (valentinaHallway != null)
            valentinaHallway.SetActive(true);
        
        if (valentinaRoom != null)
            valentinaRoom.SetActive(false);
        
        if (thinkingText != null)
        {
            thinkingCanvasGroup = thinkingText.GetComponent<CanvasGroup>();
            if (thinkingCanvasGroup == null)
                thinkingCanvasGroup = thinkingText.gameObject.AddComponent<CanvasGroup>();
            thinkingCanvasGroup.alpha = 0f;
            thinkingText.gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        if (waitingForPhotoInspection && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F pressed - completing photo inspection");
            CompletePhotoInspection();
        }
    }
    
    public void ShowThinkingTextOnBlackScreen(string message)
    {
        StartCoroutine(DisplayThinkingTextOnBlackScreen(message));
    }
    
    IEnumerator DisplayThinkingTextOnBlackScreen(string message)
    {
        if (thinkingText == null) yield break;
        
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = message;
        thinkingCanvasGroup.alpha = 1f;
        
        // Text stays for 10 seconds (handled by the black screen timer in shovel)
        yield return new WaitForSeconds(10f);
        
        thinkingText.gameObject.SetActive(false);
    }
    
    public void ShowThinkingText(string message)
    {
        StartCoroutine(DisplayThinkingText(message));
    }
    
    IEnumerator DisplayThinkingText(string message)
    {
        if (thinkingText == null) yield break;
        
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = message;
        thinkingCanvasGroup.alpha = 1f;
        
        yield return new WaitForSeconds(thinkingTextDuration);
        
        thinkingText.gameObject.SetActive(false);
    }
    
    void ShowCorrectFoodItem()
    {
        if (foodSubSandwich != null) foodSubSandwich.SetActive(false);
        if (foodChocolateBrioche != null) foodChocolateBrioche.SetActive(false);
        if (foodSpiceCupcake != null) foodSpiceCupcake.SetActive(false);
        if (foodFrostedCake != null) foodFrostedCake.SetActive(false);
        
        switch (selectedFood)
        {
            case "Sub Sandwich":
                if (foodSubSandwich != null)
                {
                    foodSubSandwich.SetActive(true);
                    currentFoodItem = foodSubSandwich;
                }
                break;
            case "Chocolate Brioche":
                if (foodChocolateBrioche != null)
                {
                    foodChocolateBrioche.SetActive(true);
                    currentFoodItem = foodChocolateBrioche;
                }
                break;
            case "Spice Cupcake":
                if (foodSpiceCupcake != null)
                {
                    foodSpiceCupcake.SetActive(true);
                    currentFoodItem = foodSpiceCupcake;
                }
                break;
            case "Frosted Cake":
                if (foodFrostedCake != null)
                {
                    foodFrostedCake.SetActive(true);
                    currentFoodItem = foodFrostedCake;
                }
                break;
            default:
                if (foodSubSandwich != null)
                {
                    foodSubSandwich.SetActive(true);
                    currentFoodItem = foodSubSandwich;
                }
                break;
        }
        
        Debug.Log("Food item shown: " + selectedFood);
    }
    
    public void ValentinaFirstDialogueComplete()
    {
        valentinaFirstTalked = true;
        
        if (valentinaHallway != null)
            valentinaHallway.SetActive(false);
        
        if (valentinaRoom != null)
        {
            valentinaRoom.SetActive(true);
            if (valentinaRoomPosition != null)
            {
                valentinaRoom.transform.position = valentinaRoomPosition.position;
                valentinaRoom.transform.rotation = valentinaRoomPosition.rotation;
            }
        }
        
        StartCoroutine(ShowEatReminderAfterDelay());
    }
    
    IEnumerator ShowEatReminderAfterDelay()
    {
        yield return new WaitForSeconds(0.6f);
        ShowEatReminder();
    }
    
    void ShowEatReminder()
    {
        if (eatReminderShown) return;
        eatReminderShown = true;
        
        StartCoroutine(ShowThinkingTextWithFade(eatReminderMessage));
        
        if (eatFoodPrompt != null)
            eatFoodPrompt.SetActive(true);
        
        if (currentFoodItem != null)
        {
            EatFoodInteraction eatScript = currentFoodItem.GetComponent<EatFoodInteraction>();
            if (eatScript != null)
            {
                eatScript.enabled = true;
            }
        }
    }
    
    public void ShowDoorEatReminder()
    {
        if (!foodEaten && !eatReminderShown)
        {
            StartCoroutine(ShowThinkingTextWithFade(eatReminderDoorMessage));
        }
    }
    
    public void RoommateTalked(string name)
    {
        Debug.Log($"RoommateTalked called: {name}");
        
        switch (name)
        {
            case "Marcelo":
                marceloTalked = true;
                break;
            case "Elio":
                elioTalked = true;
                break;
        }
    }
    
    public void EatFood()
    {
        foodEaten = true;
        
        if (eatFoodPrompt != null)
            eatFoodPrompt.SetActive(false);
        
        if (currentFoodItem != null)
            currentFoodItem.SetActive(false);
        
        Debug.Log("Player ate the " + selectedFood + "!");
    }
    
    public void ShowPhotoForInspection()
    {
        Debug.Log("ShowPhotoForInspection called - screen should be normal");
        
        // Make sure black screen is NOT active
        if (blackCanvasGroup != null)
        {
            blackCanvasGroup.alpha = 0f;
        }
        
        // Freeze player movement
        if (playerController != null)
        {
            playerController.enabled = false;
            Debug.Log("Player frozen");
        }
        
        // Show and position photo
        if (photoObject != null && mainCamera != null)
        {
            // Store original values
            originalPhotoPos = photoObject.transform.position;
            originalPhotoRot = photoObject.transform.rotation;
            originalPhotoScale = photoObject.transform.localScale;
            
            // Position photo in front of camera
            Vector3 photoPosition = mainCamera.transform.position + mainCamera.transform.forward * photoDistanceFromCamera;
            photoObject.transform.position = photoPosition;
            photoObject.transform.LookAt(mainCamera.transform);
            photoObject.transform.localScale = originalPhotoScale * 1.5f;
            
            photoObject.SetActive(true);
            Debug.Log("Photo activated on NORMAL screen at: " + photoPosition);
            
            // Start continuous rotation
            isPhotoVisible = true;
            rotationCoroutine = StartCoroutine(ContinuousRotatePhoto());
        }
        else
        {
            Debug.LogError("PhotoObject or MainCamera is null!");
            CompletePhotoInspection();
            return;
        }
        
        waitingForPhotoInspection = true;
        
        // Show prompt
        if (thinkingText != null)
        {
            StartCoroutine(ShowTempText("Press F to continue", 0.5f));
        }
    }
    
    IEnumerator ContinuousRotatePhoto()
    {
        Debug.Log("Starting continuous photo rotation");
        while (isPhotoVisible && photoObject != null && photoObject.activeSelf)
        {
            photoObject.transform.Rotate(0, 90f * Time.deltaTime, 0);
            yield return null;
        }
        Debug.Log("Photo rotation stopped");
    }
    
    IEnumerator ShowTempText(string message, float duration)
    {
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = message;
        thinkingCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(duration);
        thinkingText.gameObject.SetActive(false);
    }
    
    void CompletePhotoInspection()
    {
        Debug.Log("CompletePhotoInspection called");
        waitingForPhotoInspection = false;
        isPhotoVisible = false;
        photoInspected = true;
        
        // Stop rotation coroutine
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
            rotationCoroutine = null;
        }
        
        // Hide photo and restore position
        if (photoObject != null)
        {
            photoObject.SetActive(false);
            photoObject.transform.position = originalPhotoPos;
            photoObject.transform.rotation = originalPhotoRot;
            photoObject.transform.localScale = originalPhotoScale;
            Debug.Log("Photo hidden and restored");
        }
        
        // Unfreeze player
        if (playerController != null)
        {
            playerController.enabled = true;
            Debug.Log("Player unfrozen");
        }
        
        // Complete shovel pickup
        if (shovelScript != null)
            shovelScript.CompletePickup();
        
        // Show thinking text (5 seconds)
        StartCoroutine(PhotoFoundSequence());
    }
    
    IEnumerator PhotoFoundSequence()
    {
        yield return StartCoroutine(ShowThinkingTextWithFade(photoFoundMessage, thinkingTextDuration));
        
        AdvanceToAfternoon();
        
        if (valentinaSecondDialogue != null)
        {
            valentinaSecondDialogue.SetAsSecondDialogue();
            Debug.Log("Valentina's second dialogue is now available");
        }
    }
    
    void AdvanceToAfternoon()
    {
        timeAdvanced = true;
        Debug.Log("Time advances to 2pm");
    }
    
    IEnumerator ShowThinkingTextWithFade(string message, float duration = 3f)
    {
        if (thinkingText == null) yield break;
        
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = "";
        
        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            thinkingCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
            yield return null;
        }
        thinkingCanvasGroup.alpha = 1f;
        
        foreach (char c in message.ToCharArray())
        {
            thinkingText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        
        yield return new WaitForSeconds(duration);
        
        elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            thinkingCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
            yield return null;
        }
        thinkingCanvasGroup.alpha = 0f;
        thinkingText.gameObject.SetActive(false);
    }
    
    public bool IsValentinaTalked()
    {
        return valentinaFirstTalked;
    }
    
    public bool HasFoodEaten()
    {
        return foodEaten;
    }
    
    public bool AreAllRoommatesTalked()
    {
        return valentinaFirstTalked && marceloTalked && elioTalked;
    }
    
    public bool HasPhotoInspected()
    {
        return photoInspected;
    }
}
