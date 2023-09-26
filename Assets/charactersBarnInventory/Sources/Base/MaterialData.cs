using UnityEngine;

[CreateAssetMenu(fileName = "NewMaterialData", menuName = "ScriptableObjects/Characters Barn Inventory/Data/Material Data", order = 1)]
public class MaterialData : ItemData
{
    public MaterialData() : base(CharactersBarnInventoryEnums.ItemDataType.Material, CharactersBarnInventoryEnums.ItemActionVerb.Combine) { }

    //Properties such as quality of the material...
}
