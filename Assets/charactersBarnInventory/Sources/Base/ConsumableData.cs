using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumableData", menuName = "ScriptableObjects/Characters Barn Inventory/Data/Consumable Data", order = 3)]
public class ConsumableData : ItemData
{
    public ConsumableData() : base(CharactersBarnInventoryEnums.ItemDataType.Consumable, CharactersBarnInventoryEnums.ItemActionVerb.Consume) { }

    //Status affected, damage inflicted...
}
