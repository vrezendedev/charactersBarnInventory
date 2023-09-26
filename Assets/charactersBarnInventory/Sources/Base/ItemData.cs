using Newtonsoft.Json;
using UnityEngine;
using static CharactersBarnInventoryEnums;

public class ItemData : ScriptableObject
{
    public string Name;
    public string Description;
    public readonly ItemDataType DataType;
    [JsonIgnore] public readonly ItemActionVerb ItemActionVerb;
    public ItemData(ItemDataType dt, ItemActionVerb icv)
    {
        DataType = dt;
        ItemActionVerb = icv;
    }
}
