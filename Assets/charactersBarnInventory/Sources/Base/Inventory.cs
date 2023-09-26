using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventory", menuName = "ScriptableObjects/Characters Barn Inventory/Inventory", order = 2)]
public class Inventory : ScriptableObject
{
    public int ID;
    public string Owner;
    public int InventorySlotsLimit = 10;
    public List<ValueTuple<int, Item>> Items;

    public void Init(int id, string owner, List<ValueTuple<int, Item>> items)
    {
        Items = new List<ValueTuple<int, Item>>();

        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    public int FindItemIndex(Item item) => Items.FindIndex(obj => obj.Item2.ID == item.ID);

}
