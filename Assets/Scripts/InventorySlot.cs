using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image icon;
    public TextMeshProUGUI countText;
    public uint slotId;
    private Vector2 startPosition;
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        uint itemId = InventorySystem.Instance.GetItemInSlot(slotId);
        if (itemId == 0) return;

        startPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (originalParent == null) return;
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (originalParent == null) return;

        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = startPosition;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot fromSlot = eventData.pointerDrag.GetComponent<InventorySlot>();
        if (fromSlot != null && fromSlot != this)
        {
            InventorySystem.Instance.SwapSlots(fromSlot.slotId, slotId);
            InventoryUI.Instance.UpdateAllSlots();
        }
    }
}