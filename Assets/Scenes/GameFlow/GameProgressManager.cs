using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;
    
    private bool nunDialogueCompleted = false;
    private bool vendorDialogueCompleted = false; // NEW
    private string selectedFood = ""; // NEW - store chosen food
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameProgressManager initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        Time.timeScale = 1f;
        Debug.Log("GameProgressManager Start - Time.timeScale = " + Time.timeScale);
        
        // Load saved food if exists
        selectedFood = PlayerPrefs.GetString("SelectedFood", "");
    }
    
    public void CompleteNunDialogue()
    {
        nunDialogueCompleted = true;
        Debug.Log("=== NUN DIALOGUE COMPLETED ===");
        
        // Find and enable the PlazaTriggerText
        PlazaTriggerText plazaTrigger = FindObjectOfType<PlazaTriggerText>();
        if (plazaTrigger != null)
        {
            plazaTrigger.EnableTrigger();
            Debug.Log("PlazaTrigger enabled via GameProgressManager");
        }
        else
        {
            Debug.LogWarning("PlazaTrigger NOT found!");
        }
    }
    
    public void CompleteVendorDialogue(string food)
    {
        vendorDialogueCompleted = true;
        selectedFood = food;
        PlayerPrefs.SetString("SelectedFood", food);
        PlayerPrefs.Save();
        Debug.Log("=== VENDOR DIALOGUE COMPLETED ===");
        Debug.Log("Selected food: " + food);
    }
    
    public bool IsNunDialogueCompleted()
    {
        return nunDialogueCompleted;
    }
    
    public bool IsVendorDialogueCompleted()
    {
        return vendorDialogueCompleted;
    }
    
    public string GetSelectedFood()
    {
        return selectedFood;
    }
}
