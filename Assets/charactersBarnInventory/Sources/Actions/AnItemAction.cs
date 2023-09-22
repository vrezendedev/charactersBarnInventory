using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAnItemAction", menuName = "ScriptableObjects/Characters Barn Inventory/Actions/An Item Action", order = 0)]
public class AnItemAction : ItemAction
{
    public string Description;
    public override void Act(Inventory iv)
    {
        Debug.Log(Description);
    }
}
