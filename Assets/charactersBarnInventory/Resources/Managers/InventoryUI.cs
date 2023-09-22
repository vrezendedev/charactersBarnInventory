
using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Required Refs:")]
    public InventoryManager currentInventoryManager;

    [Header("UI Required Elements - Highly Recommended to use and customize the Library's Prefabs")]
    public GameObject inventoryPanel;
    public GameObject slotsImages;
    [SerializeField] private GameObject rows;
    private GameObject _contentPanel;

    [Header("UI Custom Elements")]
    public GameObject inventoryOpenButton = null;
    public GameObject inventoryCloseButton = null;

    [Header("Customization:")]
    public int slotsPerRow = 5;

    private float _width;
    private float _spriteSize;
    private float _distancePerSlot;

    void Awake()
    {
        if (inventoryPanel == null) return;

        if (inventoryOpenButton != null)
            inventoryOpenButton.GetComponent<Button>().onClick.AddListener(delegate { inventoryPanel.SetActive(true); });

        if (inventoryCloseButton != null)
            inventoryCloseButton.GetComponent<Button>().onClick.AddListener(delegate { inventoryPanel.SetActive(false); });

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

        Draw();
    }

    public void OnOpenAndClose() => inventoryPanel.SetActive(!inventoryPanel.activeInHierarchy);

    public void Draw()
    {
        foreach (Transform child in _contentPanel.transform)
        {
            DestroyImmediate(child.gameObject);
        }

        for (int i = 0; i < currentInventoryManager.GetInventorySlotsLimit(); i++)
        {
            var row = Instantiate(rows, _contentPanel.transform);
            for (int j = 0; j < slotsPerRow; j++, i++)
            {
                var obj = Instantiate(slotsImages, row.transform);
                var item = currentInventoryManager.
                GetItemByIndex(i);

                if (item == null) continue;

                obj.GetComponent<Image>().sprite = item.Sprite;
                obj.GetComponent<Button>().onClick.AddListener(delegate { Debug.Log(item.ItemData.Name); });
            }

        }
    }

}