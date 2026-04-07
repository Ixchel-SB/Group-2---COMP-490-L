using UnityEngine;
using TMPro;

public class DormManager : MonoBehaviour
{
    private bool valentinaTalked = false;
    private bool marceloTalked = false;
    private bool elioTalked = false;
    
    public GameObject eatFoodPrompt;
    public GameObject foodItem;
    public GameObject bedInteraction;
    
    void Start()
    {
        if (eatFoodPrompt != null)
            eatFoodPrompt.SetActive(false);
        
        if (foodItem != null)
            foodItem.SetActive(false);
        
        if (bedInteraction != null)
            bedInteraction.SetActive(false);
    }
    
    public void RoommateTalked(string name)
    {
        switch (name)
        {
            case "Valentina":
                valentinaTalked = true;
                break;
            case "Marcelo":
                marceloTalked = true;
                break;
            case "Elio":
                elioTalked = true;
                break;
        }
        
        CheckAllTalked();
    }
    
    void CheckAllTalked()
    {
        if (valentinaTalked && marceloTalked && elioTalked)
        {
            Debug.Log("All roommates talked to!");
            
            if (eatFoodPrompt != null)
                eatFoodPrompt.SetActive(true);
            
            if (foodItem != null)
                foodItem.SetActive(true);
        }
    }
    
    public void EatFood()
    {
        if (eatFoodPrompt != null)
            eatFoodPrompt.SetActive(false);
        
        if (foodItem != null)
            foodItem.SetActive(false);
        
        if (bedInteraction != null)
            bedInteraction.SetActive(true);
        
        Debug.Log("Player ate the food!");
    }
}
