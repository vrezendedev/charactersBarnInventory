using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using static CharactersBarnInventoryEnums;
using System.Collections.Generic;
using System;
using System.Linq;

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
    public GameObject discardPlusOneBtn;
    public GameObject discardPlusFiveBtn;
    public GameObject discardPlusTenBtn;
    public GameObject discardAllBtn;
    public GameObject discardResetBtn;
    public GameObject discardCancelBtn;
    public GameObject discardConfirmBtn;

    public GameObject transferMenu;
    public TextMeshProUGUI transferText;
    public TMP_Dropdown transferDropdown;
    public TMP_InputField transferInputField;
    public GameObject transferCancelBtn;
    public GameObject transferConfirmBtn;

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

    private Item _transferItem = null;
    private InventoryManager _transferTarget = null;
    private int _transferQuantity = 0;
    private int _transferMaxQuantity = 0;

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
            inventoryCloseButton.GetComponent<Button>().onClick.AddListener(delegate { CloseAll(); });


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

        discardPlusOneBtn.GetComponent<Button>().onClick.AddListener(delegate { SetDiscardQuantity(true, 1); });
        discardPlusFiveBtn.GetComponent<Button>().onClick.AddListener(delegate { SetDiscardQuantity(true, 5); });
        discardPlusTenBtn.GetComponent<Button>().onClick.AddListener(delegate { SetDiscardQuantity(true, 10); });
        discardResetBtn.GetComponent<Button>().onClick.AddListener(delegate { SetDiscardQuantity(false, 0); });
        discardCancelBtn.GetComponent<Button>().onClick.AddListener(delegate { discardMenu.SetActive(false); _discardItemName = ""; _discardQuantity = 0; _currentInventoryManager.SetSelectedItem(null); });
        transferCancelBtn.GetComponent<Button>().onClick.AddListener(delegate { transferMenu.SetActive(false); _transferTarget = null; _transferItem = null; _transferQuantity = 0; _currentInventoryManager.SetSelectedItem(null); });
        transferInputField.onValueChanged.AddListener(delegate (string e)
        {
            _transferQuantity = e.Length > 0 ? int.Parse(e) : 0;
            _transferQuantity = Mathf.Clamp(_transferQuantity, 0, _transferMaxQuantity);
            transferInputField.SetTextWithoutNotify(_transferQuantity.ToString());
            transferText.text = $"Transfer \n {_transferItem.ItemData.Name}... to:";
        });
        transferConfirmBtn.GetComponent<Button>().onClick.AddListener(delegate
        {
            if (_currentInventoryManager != null)
            {
                _currentInventoryManager.Transfer(_transferTarget, _transferQuantity, _transferItem);
                transferMenu.SetActive(false);
                _transferItem = null;
                _transferMaxQuantity = 0;
                _transferQuantity = 0;
                _currentInventoryManager.SetSelectedItem(null);
            }
        });
    }

    void Update()
    {
        if (discardMenu.activeSelf)
            discardConfirmBtn.GetComponent<Button>().interactable = _discardQuantity > 0;

        if (transferMenu.activeSelf)
            transferConfirmBtn.GetComponent<Button>().interactable = _transferQuantity > 0 && _transferTarget != null;

    }

    public void OnOpenAndClose()
    {
        if (!inventoryPanel.activeSelf)
        {
            Draw();
            inventoryPanel.SetActive(true);
        }
        else
        {
            Erase();
            CloseAll();
        }
    }

    private void CloseAll()
    {
        _currentInventoryManager.SetSelectedItem(null);
        inventoryPanel.SetActive(false);
        itemOptionsMenu.SetActive(false);
        discardMenu.SetActive(false);
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
        if (discardMenu.activeSelf || transferMenu.activeSelf) return;

        var useBtn = useButton.GetComponent<Button>();
        var dscBtn = discardButton.GetComponent<Button>();
        var trnBtn = transferButton.GetComponent<Button>();

        useBtn.onClick.RemoveAllListeners();
        dscBtn.onClick.RemoveAllListeners();
        trnBtn.onClick.RemoveAllListeners();

        transferButton.SetActive(FindObjectsOfType<InventoryManager>().Count() > 1);

        if (show)
        {
            itemOptionsMenu.SetActive(true);
            itemOptionsItemName.text = item.Item2.ItemData.Name;
            useBtn.GetComponentInChildren<TextMeshProUGUI>().text = item.Item2.ItemData.ItemActionVerb.ToString();
            useBtn.onClick.AddListener(delegate { _currentInventoryManager.InteractWithItem(item.Item2, ItemOptions.Use); });
            dscBtn.onClick.AddListener(delegate { HandleDiscardOption(item); });
            trnBtn.onClick.AddListener(delegate { HandleTransfer(item); });
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
        discardAllBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        discardAllBtn.GetComponent<Button>().onClick.AddListener(delegate { SetDiscardQuantity(false, item.Item1); });
        discardConfirmBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        discardConfirmBtn.GetComponent<Button>().onClick.AddListener(delegate { _currentInventoryManager.Discard(item.Item2, _discardQuantity); _discardItemName = ""; _discardQuantity = 0; _discardMaxQuantity = 0; discardMenu.SetActive(false); _currentInventoryManager.SetSelectedItem(null); });
        discardMenu.SetActive(true);
    }

    public void HandleTransfer(ValueTuple<int, Item> item)
    {
        _transferItem = item.Item2;
        _transferMaxQuantity = item.Item1;
        transferText.text = $"Transfer \n {_transferItem.ItemData.Name}... to:";

        itemOptionsMenu.SetActive(false);

        transferDropdown.onValueChanged.RemoveAllListeners();
        transferDropdown.ClearOptions();
        List<string> options = new List<string>();

        foreach (var inventoryManager in FindObjectsOfType<InventoryManager>())
        {
            if (inventoryManager != _currentInventoryManager)
            {
                options.Add(inventoryManager.gameObject.name);
            }
        }

        transferDropdown.onValueChanged.AddListener(delegate (int index)
        {
            _transferTarget = GameObject.Find(transferDropdown.options[index].text).GetComponent<InventoryManager>();
        });

        transferDropdown.AddOptions(options);
        //Initialize _transferTarget;
        _transferTarget = GameObject.Find(transferDropdown.options[0].text).GetComponent<InventoryManager>();
        transferMenu.SetActive(true);
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
    }

}