using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;
    
    private bool nunDialogueCompleted = false;
    
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
    }
    
    public void CompleteNunDialogue()
    {
        nunDialogueCompleted = true;
        Debug.Log("=== NUN DIALOGUE COMPLETED ===");
        
        // Find and enable the PlazaTriggerText (changed from TriggerText)
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
    
    public bool IsNunDialogueCompleted()
    {
        return nunDialogueCompleted;
    }
}
