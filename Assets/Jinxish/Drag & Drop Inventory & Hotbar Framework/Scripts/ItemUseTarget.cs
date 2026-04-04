using UnityEngine;
using InventoryFramework;

public class ItemUseTarget : MonoBehaviour
{
    public int requiredItemId;
    public HotbarUI hotbarUI;
    public Transform placementPoint;
    public GameObject placedObjectPrefab;

    public void Interact()
    {
        Item selectedItem = hotbarUI.GetSelectedItem();

        if (selectedItem == null)
        {
            Debug.Log("No item selected.");
            return;
        }

        if (selectedItem.actionType != Item.ItemActionType.UseOnTarget)
        {
            Debug.Log("Selected item is not a UseOnTarget item.");
            return;
        }

        if (selectedItem.id != requiredItemId)
        {
            Debug.Log("Wrong item.");
            return;
        }

        Debug.Log("Correct item used: " + selectedItem.itemName);

        if (placedObjectPrefab == null)
        {
            Debug.LogWarning("placedObjectPrefab is NOT assigned.");
            return;
        }

        if (placementPoint == null)
        {
            Debug.LogWarning("placementPoint is NOT assigned.");
            return;
        }

        GameObject spawnedObject = Instantiate(
            placedObjectPrefab,
            placementPoint.position,
            placementPoint.rotation
        );

        Debug.Log("Spawned: " + spawnedObject.name + " at " + placementPoint.position);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

}