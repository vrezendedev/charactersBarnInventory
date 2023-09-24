using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static CharactersBarnInventoryEnums;

public class InventoryManager : MonoBehaviour
{
    [Header("Required but Interchangeable via Event:")]
    [SerializeField] private Inventory _inventory;

    [Header("Multichar Options")]
    [Tooltip("If active Draw on UI"), SerializeField] private bool isActiveChar = false;

    private Item _selectedItem = null;
    private string _serializePath = "";

    void OnDisable()
    {
        Serialize(_serializePath, _inventory);
    }

    void Start()
    {
        if (_inventory == null) return;

        _serializePath = "inv/" + _inventory.ID + "_" + _inventory.Owner;
        LoadSerializedInventory();
    }

    public bool Add(Item item, int quantity)
    {

        if (item == null || quantity <= 0) return false;

        int index = _inventory.FindItemIndex(item);

        if (index == -1)
        {
            if (_inventory.Items.Count < _inventory.InventorySlotsLimit)
                this._inventory.Items.Add(new(quantity, item));
            else
                return false;
        }
        else
        {
            var itm = this._inventory.Items[index];
            itm.Item1 += quantity;
            this._inventory.Items[index] = itm;
        }

        DrawIfActive();
        return true;
    }

    public void Discard(Item item, int quantity)
    {
        int index = _inventory.FindItemIndex(item);
        var itm = this._inventory.Items[index];

        if (quantity < itm.Item1)
        {
            itm.Item1 -= quantity;
            _inventory.Items[index] = itm;
        }
        else
        {
            _inventory.Items.Remove(itm);
        }

        DrawIfActive();
    }

    public void Transfer(InventoryManager im, int quantity)
    {
        if (_selectedItem == null) return;

        int index = _inventory.FindItemIndex(_selectedItem);
        var item = _inventory.Items[index];

        if (quantity < item.Item1)
        {
            item.Item1 -= quantity;
            im.Add(item.Item2, item.Item1);
        }
        else
        {
            im.Add(item.Item2, item.Item1);
        }

        this.Discard(item.Item2, quantity);
    }

    public void Use()
    {
        if (_selectedItem == null) return;
        _selectedItem.ItemAction.Act(this._inventory);
        DrawIfActive();
    }

    public ValueTuple<int, Item>? GetItemByIndex(int index)
    {
        try
        {
            return this._inventory.Items[index];
        }
        catch { return null; }
    }

    public int GetInventorySlotsLimit() => this._inventory.InventorySlotsLimit;

    public void IncreaseInventorySlotsLimit(int acc)
    {
        this._inventory.InventorySlotsLimit += acc;
        DrawIfActive();
    }

    public void SetSelectedItem(Item item)
    {
        _selectedItem = item;
        if (_selectedItem != null)
            Debug.Log(item.ItemData.Name);
    }

    public void InteractWithItem(ItemOptions option)
    {
        switch (option)
        {
            case ItemOptions.Use:
                break;
            case ItemOptions.Discard:
                break;
            case ItemOptions.Transfer:

                break;
        }
    }

    private void DrawIfActive()
    {
        if (isActiveChar) InventoryUI.DrawInventory(this);
    }

    public void ChangeActiveState(bool value)
    {
        isActiveChar = value;
        DrawIfActive();
    }

    private void LoadSerializedInventory()
    {
        List<Item> items = Resources.LoadAll<Item>("Items").ToList();

        try
        {
            _inventory = Deserialize<Inventory>(_serializePath);
            for (int i = 0; i < _inventory.Items.Count; i++)
            {
                _inventory.Items[i] = new ValueTuple<int, Item>(_inventory.Items[i].Item1, items.Find(obj => obj.ID == _inventory.Items[i].Item2.ID));
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            _inventory.Init(_inventory.ID, _inventory.name, new List<(int, Item)>());
        }

        DrawIfActive();
    }

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

    private static T Deserialize<T>(string extraPath)
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
