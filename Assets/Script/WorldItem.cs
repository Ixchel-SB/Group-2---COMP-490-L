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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            Inventory inv = other.GetComponent<Inventory>();
            if (inv != null)
            {
                Pickup(inv);
            }
        }
    }
}
