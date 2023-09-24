using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using static CharactersBarnInventoryEnums;

public class InventoryUI : MonoBehaviour
{
    private InventoryManager _currentInventoryManager;

    [Header("UI Required Elements - Must use the Library's Prefabs, but you may customize its visual")]
    public GameObject inventoryPanel;
    public GameObject itemOptionsMenu;
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
        if (inventoryPanel == null) return;

        if (itemOptionsMenu == null) return;

        if (inventoryOpenButton != null)
            inventoryOpenButton.GetComponent<Button>().onClick.AddListener(delegate { inventoryPanel.SetActive(true); });

        if (inventoryCloseButton != null)
            inventoryCloseButton.GetComponent<Button>().onClick.AddListener(delegate { inventoryPanel.SetActive(false); });


        itemOptionsMenu.SetActive(false);

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
    }

    public void OnOpenAndClose()
    {
        if (!inventoryPanel.activeInHierarchy) Draw();
        else Erase();

        inventoryPanel.SetActive(!inventoryPanel.activeInHierarchy);
    }

    public void HandleDrawInventory(InventoryManager im)
    {
        if (_currentInventoryManager != null)
            _currentInventoryManager.SetSelectedItem(null);

        _currentInventoryManager = im;

        if (inventoryPanel.activeInHierarchy)
        {
            Erase();
            Draw();
        }
    }

    public void HandleItemOptionsMenu(bool show, ItemDataType? itemDataType)
    {
        if (show)
        {
            itemOptionsMenu.SetActive(true);
        }
        else
        {
            itemOptionsMenu.SetActive(false);
        }
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
                           HandleItemOptionsMenu(false, null);
                       }
                    );
                    continue;
                };

                obj.GetComponent<Image>().sprite = item.Value.Item2.Sprite;
                obj.GetComponent<Button>().onClick.AddListener(
                    delegate
                    {
                        _currentInventoryManager.SetSelectedItem(item.Value.Item2);
                        HandleItemOptionsMenu(true, item.Value.Item2.ItemData.DataType);
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