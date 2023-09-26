using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using static CharactersBarnInventoryEnums;
using System.Collections.Generic;
using System;

public class InventoryUI : MonoBehaviour
{
    private InventoryManager _currentInventoryManager;

    [Header("UI Required Elements - Must use the Library's Prefabs, but you may customize its visual")]
    public GameObject inventoryPanel;

    public GameObject itemOptionsMenu;
    public TextMeshProUGUI itemOptionsItemName;
    public GameObject useButton;
    public GameObject discardButton;
    public GameObject transferButton;

    public GameObject discardMenu;
    public TextMeshProUGUI discardText;
    public GameObject discardPlusOne;
    public GameObject discardPlusFive;
    public GameObject discardPlusTen;
    public GameObject discardAll;
    public GameObject discardReset;
    public GameObject discardCancel;
    public GameObject discardConfirm;

    public GameObject slotsImages;
    [SerializeField] private GameObject rows;
    private GameObject _contentPanel;

    [Header("UI Custom and Nonobligatory Elements")]
    public GameObject inventoryOpenButton = null;
    public GameObject inventoryCloseButton = null;

    [Header("Customization:")]
    public int slotsPerRow = 5;

    public static UnityAction<InventoryManager> DrawInventory;

    private float _width;
    private float _spriteSize;
    private float _distancePerSlot;
    private int _discardQuantity = 0;
    private int _discardMaxQuantity = 0;
    private string _discardItemName = "";

    void OnEnable()
    {
        DrawInventory += HandleDrawInventory;
    }

    void OnDisable()
    {
        DrawInventory -= HandleDrawInventory;
    }

    void Awake()
    {
        if (inventoryOpenButton != null)
            inventoryOpenButton.GetComponent<Button>().onClick.AddListener(delegate { inventoryPanel.SetActive(true); });

        if (inventoryCloseButton != null)
            inventoryCloseButton.GetComponent<Button>().onClick.AddListener(delegate { inventoryPanel.SetActive(false); });


        itemOptionsMenu.SetActive(false);
        discardMenu.SetActive(false);

        _contentPanel = inventoryPanel.GetComponent<ScrollRect>().content.gameObject;

        var size = _contentPanel.GetComponent<RectTransform>().sizeDelta;
        _width = size.x;

        float rowDistance = _spriteSize = _width / slotsPerRow;
        _spriteSize -= _spriteSize * 0.05f;
        slotsImages.GetComponent<RectTransform>().sizeDelta = new Vector2(_spriteSize, _spriteSize);

        _distancePerSlot = Mathf.Abs(_spriteSize * slotsPerRow - _width) / slotsPerRow;

        var rt = rows.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, rowDistance);

        var hlg = rows.GetComponent<HorizontalLayoutGroup>();
        var vlg = _contentPanel.GetComponent<VerticalLayoutGroup>();
        hlg.spacing = vlg.spacing = Mathf.FloorToInt(_distancePerSlot);
        vlg.padding.top = vlg.padding.left = (int)vlg.spacing * 2;

        discardPlusOne.GetComponent<Button>().onClick.AddListener(delegate { SetDiscardQuantity(true, 1); });
        discardPlusFive.GetComponent<Button>().onClick.AddListener(delegate { SetDiscardQuantity(true, 5); });
        discardPlusTen.GetComponent<Button>().onClick.AddListener(delegate { SetDiscardQuantity(true, 10); });
        discardReset.GetComponent<Button>().onClick.AddListener(delegate { SetDiscardQuantity(false, 0); });
        discardCancel.GetComponent<Button>().onClick.AddListener(delegate { discardMenu.SetActive(false); _discardItemName = ""; _discardQuantity = 0; });
    }

    void Update()
    {
        if (discardMenu.activeSelf)
            discardConfirm.GetComponent<Button>().interactable = _discardQuantity > 0;

    }

    public void OnOpenAndClose()
    {
        if (!inventoryPanel.activeSelf) Draw();
        else Erase();

        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    private void SetDiscardQuantity(bool acc, int i)
    {
        if (acc)
        {
            _discardQuantity += i;
            _discardQuantity = Math.Clamp(_discardQuantity, 0, _discardMaxQuantity);
        }

        else _discardQuantity = i;

        discardText.text = $"Discard: {_discardQuantity} of {_discardItemName}";
    }

    public void HandleDrawInventory(InventoryManager im)
    {
        if (_currentInventoryManager != null)
            _currentInventoryManager.SetSelectedItem(null);

        _currentInventoryManager = im;

        if (inventoryPanel.activeSelf)
        {
            Erase();
            Draw();
        }
    }

    public void HandleItemOptionsMenu(bool show, ValueTuple<int, Item> item = new ValueTuple<int, Item>())
    {
        var useBtn = useButton.GetComponent<Button>();
        var dscBtn = discardButton.GetComponent<Button>();
        var trnBtn = transferButton.GetComponent<Button>();

        useBtn.onClick.RemoveAllListeners();
        dscBtn.onClick.RemoveAllListeners();
        trnBtn.onClick.RemoveAllListeners();

        if (discardMenu.activeInHierarchy) return;

        if (show)
        {
            itemOptionsMenu.SetActive(true);
            itemOptionsItemName.text = item.Item2.ItemData.Name;
            useBtn.GetComponentInChildren<TextMeshProUGUI>().text = item.Item2.ItemData.ItemActionVerb.ToString();
            useBtn.onClick.AddListener(delegate { _currentInventoryManager.InteractWithItem(item.Item2, ItemOptions.Use); });
            dscBtn.onClick.AddListener(delegate { HandleDiscardOption(item); });
            trnBtn.onClick.AddListener(delegate { _currentInventoryManager.InteractWithItem(item.Item2, ItemOptions.Transfer); });
        }
        else
        {
            itemOptionsMenu.SetActive(false);
            itemOptionsItemName.text = "";
        }
    }

    public void HandleDiscardOption(ValueTuple<int, Item> item)
    {
        _discardItemName = item.Item2.ItemData.Name;
        _discardMaxQuantity = item.Item1;
        discardText.text = $"Discard: {_discardQuantity} of {_discardItemName}";
        itemOptionsMenu.SetActive(false);
        discardAll.GetComponent<Button>().onClick.RemoveAllListeners();
        discardAll.GetComponent<Button>().onClick.AddListener(delegate { SetDiscardQuantity(false, item.Item1); });
        discardConfirm.GetComponent<Button>().onClick.RemoveAllListeners();
        discardConfirm.GetComponent<Button>().onClick.AddListener(delegate { _currentInventoryManager.Discard(item.Item2, _discardQuantity); _discardItemName = ""; _discardQuantity = 0; _discardMaxQuantity = 0; discardMenu.SetActive(false); });
        discardMenu.SetActive(true);
    }

    public void Draw()
    {
        for (int i = 0; i < _currentInventoryManager.GetInventorySlotsLimit(); i++)
        {
            var row = Instantiate(rows, _contentPanel.transform);
            for (int j = 0; j < slotsPerRow; j++, i++)
            {
                var obj = Instantiate(slotsImages, row.transform);
                var item = _currentInventoryManager.GetItemByIndex(i);

                if (item == null)
                {
                    obj.GetComponent<Button>().onClick.AddListener(
                       delegate
                       {
                           _currentInventoryManager.SetSelectedItem(null);
                           HandleItemOptionsMenu(false);
                       }
                    );
                    continue;
                };

                obj.GetComponent<Image>().sprite = item.Value.Item2.Sprite;
                obj.GetComponent<Button>().onClick.AddListener(
                    delegate
                    {
                        if (item.Value.Item2.ID != _currentInventoryManager.GetSelectedItem()?.ID)
                        {
                            _currentInventoryManager.SetSelectedItem(item.Value.Item2);
                            HandleItemOptionsMenu(true, (ValueTuple<int, Item>)item);
                        }
                        else
                        {
                            _currentInventoryManager.SetSelectedItem(null);
                            HandleItemOptionsMenu(false);
                        }
                    }
                );
                obj.GetComponentInChildren<TextMeshProUGUI>().text = item.Value.Item1.ToString();
            }

        }
    }

    public void Erase()
    {
        for (int i = 0; i < _contentPanel.transform.childCount; i++)
        {
            var row = _contentPanel.transform.GetChild(i).gameObject;
            for (int j = 0; j < row.transform.childCount; j++)
            {
                row.transform.GetChild(j).GetComponent<Button>().onClick.RemoveAllListeners();
            }
            Destroy(row);
        }

        itemOptionsMenu.SetActive(false);
    }

}