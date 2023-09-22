using UnityEngine;
using static CharactersBarnInventoryEnums;

public class ItemData : ScriptableObject
{
    public string Name;
    public string Description;
    public readonly ItemDataType DataType;
    public ItemData(ItemDataType dt)
    {
        DataType = dt;
    }
}
