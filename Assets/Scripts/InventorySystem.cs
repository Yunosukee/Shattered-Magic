using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    public Dictionary<uint, int> items = new Dictionary<uint, int>();
    public Dictionary<uint, uint> slotItems = new Dictionary<uint, uint>();

    void Awake()
    {
        Instance = this;
        Debug.Log("[InventorySystem] Initialized");
    }

    public void AddItem(uint itemId, int amount)
    {
        if (!items.ContainsKey(itemId))
        {
            items[itemId] = 0;
            uint slotId = GetFirstEmptySlot();
            slotItems[slotId] = itemId;
            Debug.Log($"[InventorySystem] New item {itemId} assigned to slot {slotId}");
        }

        items[itemId] += amount;
        uint existingSlot = GetSlotForItem(itemId);
        InventoryEvents.ItemCountChanged(itemId, existingSlot, items[itemId]);
        Debug.Log($"[InventorySystem] Added {amount} of item {itemId}, total: {items[itemId]}");
    }

    private uint GetSlotForItem(uint itemId)
    {
        foreach (var slot in slotItems)
        {
            if (slot.Value == itemId)
                return slot.Key;
        }
        return GetFirstEmptySlot();
    }

    public void RemoveItems(uint itemId, int amount)
    {
        if (items.ContainsKey(itemId))
        {
            items[itemId] -= amount;
            if (items[itemId] <= 0)
            {
                items.Remove(itemId);
                var slotToRemove = slotItems.FirstOrDefault(x => x.Value == itemId).Key;
                slotItems.Remove(slotToRemove);
                Debug.Log($"[InventorySystem] Removed item {itemId} from slot {slotToRemove}");
            }
        }
    }

    public void SwapSlots(uint fromSlot, uint toSlot)
    {
        if (slotItems.ContainsKey(fromSlot))
        {
            uint fromItemId = slotItems[fromSlot];
            uint toItemId = slotItems.ContainsKey(toSlot) ? slotItems[toSlot] : 0;

            if (toItemId == 0)
            {
                slotItems.Remove(fromSlot);
                slotItems[toSlot] = fromItemId;
            }
            else
            {
                slotItems[fromSlot] = toItemId;
                slotItems[toSlot] = fromItemId;
            }

            if (fromItemId != 0)
                InventoryEvents.ItemCountChanged(fromItemId, toSlot, items[fromItemId]);
            if (toItemId != 0)
                InventoryEvents.ItemCountChanged(toItemId, fromSlot, items[toItemId]);

            Debug.Log($"[InventorySystem] Swapped slots {fromSlot} and {toSlot}");
        }
    }

    public uint GetItemInSlot(uint slotId)
    {
        return slotItems.ContainsKey(slotId) ? slotItems[slotId] : 0;
    }

    public int GetItemAmount(uint itemId)
    {
        return items.ContainsKey(itemId) ? items[itemId] : 0;
    }

    public bool HasEnough(uint itemId, int amount)
    {
        return GetItemAmount(itemId) >= amount;
    }


    private uint GetFirstEmptySlot()
    {
        for (uint slot = 0; slot < 24; slot++)
        {
            if (!slotItems.ContainsKey(slot))
                return slot;
        }
        return uint.MaxValue;
    }
}