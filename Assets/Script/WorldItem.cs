using UnityEngine;
using StarterAssets;

public class WorldItem : MonoBehaviour
{
    public Item item;

    private bool playerInRange;
    private Inventory playerInventory;
    private StarterAssetsInputs inputs;
    private bool pickedUp;

    private void Update()
    {
        if (!playerInRange || pickedUp) return;
        if (inputs == null || playerInventory == null) return;

        if (inputs.interact)
        {
            inputs.interact = false; // IMPORTANT: consume the press

            if (playerInventory.AddItem(item))
            {
                pickedUp = true;
                Debug.Log("Picked up: " + item.itemID);
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        playerInventory = other.GetComponentInParent<Inventory>();
        inputs = other.GetComponentInParent<StarterAssetsInputs>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        playerInventory = null;
        inputs = null;
    }
}