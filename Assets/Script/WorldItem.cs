using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public Item item;
    private bool pickedUp = false;

    public void Pickup(Inventory inventory)
    {
        if (pickedUp) return;

        if (inventory.AddItem(item))
        {
            pickedUp = true;
            gameObject.SetActive(false);
        }
    }
}
