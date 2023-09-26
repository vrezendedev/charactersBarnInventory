using System;
using UnityEngine;

public abstract class ItemAction : ScriptableObject
{
    public abstract void Act(Item item, InventoryManager iv);
}
