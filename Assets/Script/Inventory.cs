using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public List<Item> itemDatabase;

    public List<string> ownedItemIDs = new List<string>();

    public bool AddItem(Item item)
    {
        if (ownedItemIDs.Contains(item.itemID))
        return false;

        ownedItemIDs.Add(item.itemID);
        return true;
    }

    public bool HasItem(string itemID)
    {
        return ownedItemIDs.Contains(itemID);
    }

    public void RemoveItem(string itemID)
    {
        ownedItemIDs.Remove(itemID);
    }

}
