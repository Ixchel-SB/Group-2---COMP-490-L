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
    private bool secondDialogueCompleted = false;  // ADD THIS
    
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
    public float thinkingTextDuration = 3f;
    
    [Header("Backyard")]
    public GameObject graveInteraction;
    public GameObject shovelInteraction;
    public GameObject photoObject;
    public float photoDistanceFromCamera = 2f;
    public float photoRotationSpeed = 2f;
    public GameObject blackScreenPanel;
    
    [Header("Valentina Second Dialogue")]
    public RoommateDialogue valentinaSecondDialogue;
    
    [Header("Post Photo Sequence")]
    public PostPhotoSequence postPhotoSequence;
    
    [Header("Exit Door")]
    public DormDoorInteraction exitDoor;
    
    private CanvasGroup thinkingCanvasGroup;
    private bool waitingForPhotoInspection = false;
    private ShovelInteraction shovelScript;
    private GameObject player;
    private GameObject playerModel;
    private MonoBehaviour playerController;
    private Camera mainCamera;
    private Vector3 originalPhotoPos;
    private Quaternion originalPhotoRot;
    private Vector3 originalPhotoScale;
    private CanvasGroup blackCanvasGroup;
    private float rotationX = 0f;
    private float rotationY = 0f;
    
    void Start()
    {
        Time.timeScale = 1f;
        
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = Camera.main;
        
        if (player != null)
        {
            // Find player model for hiding
            SkinnedMeshRenderer skinnedMesh = player.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMesh != null)
                playerModel = skinnedMesh.gameObject;
            else
                playerModel = player;
            
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
        {
            valentinaRoom.SetActive(true);
            
            // DISABLE second dialogue interaction at start (will be enabled after photo)
            RoommateInteraction valentinaInteraction = valentinaRoom.GetComponent<RoommateInteraction>();
            if (valentinaInteraction != null)
            {
                valentinaInteraction.enabled = false;
                Debug.Log("Valentina's second dialogue interaction DISABLED at start");
            }
            
            if (valentinaRoomPosition != null)
            {
                valentinaRoom.transform.position = valentinaRoomPosition.position;
                valentinaRoom.transform.rotation = valentinaRoomPosition.rotation;
            }
        }
        
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
        if (waitingForPhotoInspection)
        {
            // Handle rotation with mouse movement (no click needed)
            Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            
            if (mouseDelta != Vector2.zero && photoObject != null)
            {
                rotationY += mouseDelta.x * photoRotationSpeed;
                rotationX += -mouseDelta.y * photoRotationSpeed;
                
                // Clamp vertical rotation to prevent flipping
                rotationX = Mathf.Clamp(rotationX, -90f, 90f);
                
                // Apply rotation
                photoObject.transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
            }
            
            // Press F to continue
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("F pressed - completing photo inspection");
                StartCoroutine(CompletePhotoInspection());
            }
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
        
        yield return new WaitForSeconds(3f);
        
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
    
    // ADD THIS METHOD
    public void OnSecondDialogueCompleted()
    {
        secondDialogueCompleted = true;
        Debug.Log("Second dialogue completed - flag set to true");
    }
    
    public void ShowPhotoForInspection()
    {
        Debug.Log("ShowPhotoForInspection called");
        
        // Reset rotation values
        rotationX = 0f;
        rotationY = 0f;
        
        // Make sure black screen is NOT active
        if (blackCanvasGroup != null)
        {
            blackCanvasGroup.alpha = 0f;
        }
        
        // Hide player character
        if (playerModel != null)
        {
            playerModel.SetActive(false);
            Debug.Log("Player character hidden");
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
            
            // Position photo closer to camera (2 units away)
            Vector3 photoPosition = mainCamera.transform.position + mainCamera.transform.forward * photoDistanceFromCamera;
            photoObject.transform.position = photoPosition;
            photoObject.transform.LookAt(mainCamera.transform);
            photoObject.transform.localScale = originalPhotoScale * 1.5f;
            
            // Store initial rotation
            Vector3 initialEuler = photoObject.transform.eulerAngles;
            rotationX = initialEuler.x;
            rotationY = initialEuler.y;
            
            photoObject.SetActive(true);
            Debug.Log($"Photo activated at distance {photoDistanceFromCamera} from camera: {photoPosition}");
        }
        else
        {
            Debug.LogError("PhotoObject or MainCamera is null!");
            StartCoroutine(CompletePhotoInspection());
            return;
        }
        
        waitingForPhotoInspection = true;
        isPhotoVisible = true;
        
        // Show prompt
        if (thinkingText != null)
        {
            StartCoroutine(ShowTempText("Move your mouse to rotate the photo\nPress F to continue", 2f));
        }
    }
    
    IEnumerator ShowTempText(string message, float duration)
    {
        thinkingText.gameObject.SetActive(true);
        thinkingText.text = message;
        thinkingCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(duration);
        thinkingText.gameObject.SetActive(false);
    }
    
    IEnumerator CompletePhotoInspection()
    {
        Debug.Log("CompletePhotoInspection called - starting black screen transition");
        
        waitingForPhotoInspection = false;
        isPhotoVisible = false;
        
        // Hide photo and restore position
        if (photoObject != null)
        {
            photoObject.SetActive(false);
            photoObject.transform.position = originalPhotoPos;
            photoObject.transform.rotation = originalPhotoRot;
            photoObject.transform.localScale = originalPhotoScale;
            Debug.Log("Photo hidden and restored");
        }
        
        // Fade to black
        if (blackCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                blackCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
                yield return null;
            }
            blackCanvasGroup.alpha = 1f;
            Debug.Log("Faded to black for thinking text");
        }
        
        // Show thinking text on black screen for 3 seconds
        if (thinkingText != null)
        {
            thinkingText.gameObject.SetActive(true);
            thinkingText.text = photoFoundMessage;
            thinkingCanvasGroup.alpha = 1f;
            Debug.Log("Showing thinking text on black screen: " + photoFoundMessage);
        }
        
        // Wait 3 seconds
        yield return new WaitForSecondsRealtime(3f);
        
        // Hide thinking text
        if (thinkingText != null)
        {
            thinkingText.gameObject.SetActive(false);
        }
        
        // Fade back to normal
        if (blackCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                blackCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
                yield return null;
            }
            blackCanvasGroup.alpha = 0f;
            Debug.Log("Faded back to normal after thinking text");
        }
        
        // Show player character again
        if (playerModel != null)
        {
            playerModel.SetActive(true);
            Debug.Log("Player character shown again");
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
        
        photoInspected = true;
        
        AdvanceToAfternoon();
        
        // Enable Valentina's second dialogue (player must talk to her next)
        if (valentinaSecondDialogue != null)
        {
            valentinaSecondDialogue.SetAsSecondDialogue();
            Debug.Log("Valentina's second dialogue is now available");
        }
        
        // ENABLE the interaction on Valentina_Room (second model)
        if (valentinaRoom != null)
        {
            RoommateInteraction valentinaInteraction = valentinaRoom.GetComponent<RoommateInteraction>();
            if (valentinaInteraction != null)
            {
                valentinaInteraction.enabled = true;
                Debug.Log("Valentina's second dialogue interaction ENABLED");
            }
        }
        
        // Reset second dialogue flag
        secondDialogueCompleted = false;
        
        // Wait for the second dialogue to be completed
        Debug.Log("Waiting for player to complete Valentina's second dialogue...");
        yield return new WaitUntil(() => secondDialogueCompleted);
        
        // Start the post-photo sequence after second dialogue ends
        if (postPhotoSequence != null)
        {
            Debug.Log("=== SECOND DIALOGUE COMPLETED - STARTING POST-PHOTO SEQUENCE ===");
            
            if (exitDoor != null)
            {
                postPhotoSequence.SetExitDoor(exitDoor);
                Debug.Log("Exit door passed to PostPhotoSequence");
            }
            
            postPhotoSequence.StartSequence();
        }
        else
        {
            Debug.LogError("PostPhotoSequence is NULL in DormManager! Make sure it's assigned.");
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
