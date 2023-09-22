using System;
using System.IO;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Required:")]
    [SerializeField] private Inventory inventory;

    private Item _selectedItem = null;
    public Item Test;
    private string _serielizePath = "";

    void Awake()
    {
        if (inventory == null) return;

        _serielizePath = "inv/" + inventory.ID + "_" + inventory.Owner;

        try
        {
            inventory = Deserialize<Inventory>(_serielizePath);
            // TO DO - Collect complete info from serialized Scriptable Objects
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            inventory.Init(inventory.ID, inventory.name, new System.Collections.Generic.List<(int, Item)>());
        }
    }

    public bool Add(Item item, int quantity)
    {
        int index = inventory.FindItemIndex(item);

        if (quantity <= 0) return false;

        if (index == -1)
        {
            if (inventory.Items.Count < inventory.InventorySlotsLimit)
                this.inventory.Items.Add(new(quantity, item));
            else
                return false;
        }
        else
        {
            var itm = this.inventory.Items[index];
            itm.Item1 += quantity;
            this.inventory.Items[index] = itm;
        }

        Serialize(_serielizePath, inventory);
        return true;
    }

    /// <summary>
    /// Discard n quantity of the target item from the inventory.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="quantity">Default -1 or max value to Discard all</param>
    public void Discard(Item item, int quantity = -1)
    {
        int index = inventory.FindItemIndex(item);
        var itm = this.inventory.Items[index];

        if (quantity > -1 && quantity < itm.Item1)
        {
            itm.Item1 -= quantity;
            inventory.Items[index] = itm;
        }
        else
            inventory.Items.Remove(itm);


        Serialize(_serielizePath, inventory);
    }

    /// <summary>
    /// Transfer n of the selected item between inventories, the one who wants to transfer should call this method targeting the other's inventory.
    /// </summary>
    /// <param name="im">Target Inventory Manager</param>
    /// <param name="quantity">Default -1 or max value to Transfer all</param>
    public void Transfer(InventoryManager im, int quantity = -1)
    {
        if (_selectedItem == null) return;

        int index = inventory.FindItemIndex(_selectedItem);
        var item = inventory.Items[index];

        if (quantity > -1 && quantity < item.Item1)
        {
            item.Item1 -= quantity;
            im.Add(item.Item2, item.Item1);
        }
        else
        {
            im.Add(item.Item2, item.Item1);
        }

        this.Discard(item.Item2, quantity != -1 ? quantity : -1);
    }

    public void Use()
    {
        if (_selectedItem == null) return;
        _selectedItem.ItemAction.Act(this.inventory);

    }

    public void IncreaseInventorySlotsLimit(int acc)
    {
        this.inventory.InventorySlotsLimit += acc;
        Serialize(_serielizePath, inventory);
    }

    public int GetInventorySlotsLimit() => this.inventory.InventorySlotsLimit;

    public Item GetItemByIndex(int index)
    {
        try
        {
            return this.inventory.Items[index].Item2;
        }
        catch { return null; }
    }

    public void InteractWithItem()
    {

    }


    //Temporary Serialize Services - the idea is to serialize only before game is finished and deserialize at the start of the game.
    private bool Serialize<T>(string extraPath, T serializable)
    {
        string path = Application.persistentDataPath + "/" + extraPath + ".json";

        try
        {
            if (File.Exists(path))
                File.Delete(path);

            FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(serializable));
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return false;
        }
    }

    public static T Deserialize<T>(string extraPath)
    {
        string path = Application.persistentDataPath + "/" + extraPath + ".json";

        if (!File.Exists(path)) throw new FileNotFoundException($"File with path [{path}] not found.");

        try
        {
            T data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            return data;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            throw ex;
        }
    }

}
