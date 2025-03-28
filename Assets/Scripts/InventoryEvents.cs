using UnityEngine;

public static class InventoryEvents
{
    public delegate void ItemCountChangedHandler(uint itemId, uint slotId, int count);
    public static event ItemCountChangedHandler OnItemCountChanged;

    public static void ItemCountChanged(uint itemId, uint slotId, int count)
    {
        Debug.Log($"InventoryEvents: Item {itemId} in slot {slotId} count: {count}");
        OnItemCountChanged?.Invoke(itemId, slotId, count);
    }
}