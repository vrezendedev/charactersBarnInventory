using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/Characters Barn Inventory/Item", order = 4)]
public class Item : ScriptableObject
{
    public int ID;
    [JsonIgnore] public CharactersBarnInventoryEnums.ItemActionType ActionType;
    [JsonIgnore] public Sprite Sprite;
    public ItemData ItemData;
    [JsonIgnore] public ItemAction ItemAction;
}
