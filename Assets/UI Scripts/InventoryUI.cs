using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;
    private Dictionary<uint, (Image icon, TextMeshProUGUI count)> itemSlots = new Dictionary<uint, (Image icon, TextMeshProUGUI count)>();
    public static InventoryUI Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        Debug.Log("[InventoryUI] Initialized");
    }

    void Start()
    {
        CreateInventoryDisplay();
        UpdateAllSlots();
        Debug.Log("[InventoryUI] Display created");
    }

    void OnEnable()
    {
        InventoryEvents.OnItemCountChanged += UpdateItemCount;
        UpdateAllSlots(); // Refresh when tab becomes active
    }

    void OnDisable()
    {
        InventoryEvents.OnItemCountChanged -= UpdateItemCount;
    }

    public void UpdateAllSlots()
    {
        for (uint slot = 0; slot < 24; slot++)
        {
            uint itemId = InventorySystem.Instance.GetItemInSlot(slot);
            if (itemId != 0)
            {
                UpdateSlotVisual(slot, itemId, InventorySystem.Instance.GetItemAmount(itemId));
            }
            else
            {
                ClearSlot(slot);
            }
        }
    }

    private void UpdateSlotVisual(uint slotId, uint itemId, int count)
    {
        if (itemSlots.ContainsKey(slotId))
        {
            var slot = itemSlots[slotId];
            slot.count.text = count.ToString();

            if (itemDatabase != null)
            {
                Sprite itemSprite = itemDatabase.GetItemSprite(itemId);
                if (itemSprite != null)
                {
                    slot.icon.sprite = itemSprite;
                    slot.icon.color = Color.white;
                }
            }
        }
    }

    private void ClearSlot(uint slotId)
    {
        if (itemSlots.ContainsKey(slotId))
        {
            var slot = itemSlots[slotId];
            slot.icon.sprite = null;
            slot.icon.color = new Color(1, 1, 1, 0);
            slot.count.text = "";
        }
    }

    private void UpdateItemCount(uint itemId, uint slotId, int count)
    {
        if (itemSlots.ContainsKey(slotId))
        {
            UpdateSlotVisual(slotId, itemId, count);
        }
    }

    void CreateInventoryDisplay()
    {
        GameObject gridObject = new GameObject("InventoryGrid");
        gridObject.transform.SetParent(transform, false);

        RectTransform gridRect = gridObject.AddComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.1f, 0.1f);
        gridRect.anchorMax = new Vector2(0.9f, 0.9f);
        gridRect.offsetMin = Vector2.zero;
        gridRect.offsetMax = Vector2.zero;

        GridLayoutGroup grid = gridObject.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(80, 80);
        grid.spacing = new Vector2(5, 5);
        grid.padding = new RectOffset(10, 10, 10, 10);
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.UpperLeft;

        Image backgroundImage = gridObject.AddComponent<Image>();
        backgroundImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

        for (int i = 0; i < 24; i++)
        {
            CreateInventorySlot(gridObject.transform, i);
        }
    }


    void CreateInventorySlot(Transform parent, int index)
    {
        GameObject slot = new GameObject($"Slot_{index}");
        slot.transform.SetParent(parent, false);

        // Add RectTransform first
        RectTransform rectTransform = slot.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(80, 80);

        // Add InventorySlot component and set its properties
        InventorySlot slotComponent = slot.AddComponent<InventorySlot>();
        slotComponent.slotId = (uint)index;

        // Setup visuals
        Image background = slot.AddComponent<Image>();
        background.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(slot.transform, false);
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.1f, 0.1f);
        iconRect.anchorMax = new Vector2(0.9f, 0.9f);
        iconRect.sizeDelta = Vector2.zero;
        Image icon = iconObj.AddComponent<Image>();
        icon.color = new Color(1f, 1f, 1f, 0f);

        GameObject textObj = new GameObject("Count");
        textObj.transform.SetParent(slot.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>(); // Add RectTransform first
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.offsetMin = new Vector2(0, 5);
        textRect.offsetMax = new Vector2(-5, 0);

        TextMeshProUGUI countText = textObj.AddComponent<TextMeshProUGUI>();
        countText.fontSize = 14;
        countText.alignment = TextAlignmentOptions.BottomRight;
        countText.color = Color.white;

        // Assign references to slot component
        slotComponent.icon = icon;
        slotComponent.countText = countText;

        // Add to dictionary
        itemSlots[(uint)index] = (icon, countText);
    }
}