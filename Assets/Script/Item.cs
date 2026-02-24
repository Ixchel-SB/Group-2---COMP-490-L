using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemID;
    public string itemName;
    public Sprite icon;

    [TextArea]
    public string description;
}

