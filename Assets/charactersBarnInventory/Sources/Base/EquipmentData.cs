using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipmentData", menuName = "ScriptableObjects/Characters Barn Inventory/Data/Equipment Data", order = 2)]
public class EquipmentData : ItemData
{
    public EquipmentData() : base(CharactersBarnInventoryEnums.ItemDataType.Equipment, CharactersBarnInventoryEnums.ItemActionVerb.Equip) { }

    //Player status bonuses and penalties
}
