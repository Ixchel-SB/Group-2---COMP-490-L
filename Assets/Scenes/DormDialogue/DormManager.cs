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
    private bool timeAdvanced = false;
    private string selectedFood = "";
    private bool eatReminderShown = false;
    
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
    public string internalMonologue = "Who is this man in the photo? Why is he with my sister?";
    public string valentinaQuestion = "I wonder if Valentina might know who this is...";
    
    [Header("Backyard")]
    public GameObject graveInteraction;
    public GameObject shovelInteraction;
    public GameObject photoObject;
    public float photoRotationDuration = 3f;
    
    [Header("Valentina Second Dialogue")]
    public RoommateDialogue valentinaSecondDialogue;
    
    private CanvasGroup thinkingCanvasGroup;
    
    void Start()
    {
        Time.timeScale = 1f;
        
        selectedFood = PlayerPrefs.GetString("SelectedFood", "");
        Debug.Log("Player selected food: " + selectedFood);
        
        ShowCorrectFoodItem();
        
        if (eatFoodPrompt != null)
            eatFoodPrompt.SetActive(false);
        
        if (graveInteraction != null)
            graveInteraction.SetActive(false);
        
        if (shovelInteraction != null)
            shovelInteraction.SetActive(false);
        
        if (photoObject != null)
            photoObject.SetActive(false);
        
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
        
        StartCoroutine(ShowThinkingText(eatReminderMessage));
        
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
            StartCoroutine(ShowThinkingText(eatReminderDoorMessage));
        }
    }
    
    public void RoommateTalked(string name)
    {
        switch (name)
        {
            case "Marcelo":
                marceloTalked = true;
                break;
            case "Elio":
                elioTalked = true;
                break;
        }
        
        CheckBackyardAccess();
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
    
    void CheckBackyardAccess()
    {
        if (marceloTalked && elioTalked)
        {
            EnableBackyard();
        }
    }
    
    void EnableBackyard()
    {
        Debug.Log("Backyard now accessible - shovel and grave enabled");
        
        if (shovelInteraction != null)
            shovelInteraction.SetActive(true);
        
        if (graveInteraction != null)
            graveInteraction.SetActive(true);
    }
    
    public void OnShovelPickedUp()
    {
        shovelPickedUp = true;
        Debug.Log("Shovel picked up - can now dig at grave");
        
        if (graveInteraction != null)
        {
            GraveInteraction grave = graveInteraction.GetComponent<GraveInteraction>();
            if (grave != null)
                grave.SetCanDig(true);
        }
    }
    
    public void OnPhotoFound()
    {
        photoFound = true;
        Debug.Log("OnPhotoFound called - showing photo and starting sequence");
        
        if (photoObject != null)
        {
            photoObject.SetActive(true);
            StartCoroutine(RotatePhoto());
        }
        else
        {
            Debug.LogError("PhotoObject is null in DormManager!");
        }
        
        StartCoroutine(PhotoFoundSequence());
    }
    
    IEnumerator RotatePhoto()
    {
        if (photoObject == null) yield break;
        
        Quaternion startRotation = photoObject.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 360f, 0);
        
        float elapsed = 0f;
        while (elapsed < photoRotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / photoRotationDuration;
            photoObject.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }
        
        photoObject.transform.rotation = endRotation;
        Debug.Log("Photo rotation complete");
    }
    
    IEnumerator PhotoFoundSequence()
    {
        yield return StartCoroutine(ShowThinkingText(internalMonologue));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ShowThinkingText(valentinaQuestion));
        
        // Time advances to 2pm
        AdvanceToAfternoon();
        
        if (valentinaSecondDialogue != null)
        {
            valentinaSecondDialogue.SetAsSecondDialogue();
            Debug.Log("Valentina's second dialogue is now available in girls room");
        }
    }
    
    void AdvanceToAfternoon()
    {
        timeAdvanced = true;
        Debug.Log("Time advances to 2pm - Valentina waiting in girls room");
    }
    
    IEnumerator ShowThinkingText(string message)
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
        
        yield return new WaitForSeconds(3f);
        
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
    
    public bool HasPhotoFound()
    {
        return photoFound;
    }
}
