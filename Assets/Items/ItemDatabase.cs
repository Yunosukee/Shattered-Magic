using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items = new List<ItemData>();
    private static ItemDatabase instance;
    public static ItemDatabase Instance
    {
        get => instance;
        set => instance = value;
    }

    void OnEnable()
    {
        Instance = this;
        Debug.Log($"[ItemDatabase] Ready - Items count: {items.Count}");
    }

    public ItemData GetItem(uint itemId)
    {
        var item = items.Find(x => x.id == itemId);
        Debug.Log($"Getting item with ID {itemId}. Found: {item?.name ?? "null"}");
        Debug.Log($"Total items in database: {items.Count}");
        Debug.Log($"Available IDs: {string.Join(", ", items.Select(i => i.id))}");
        return item;
    }

    private Sprite defaultSprite;

    private Sprite CreateDefaultSprite()
    {
        Texture2D texture = new Texture2D(32, 32);
        Color[] colors = new Color[32 * 32];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }
        texture.SetPixels(colors);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }

    public Sprite GetItemSprite(uint itemId)
    {
        var item = items.Find(x => x.id == itemId);
        if (item?.icon == null)
        {
            if (defaultSprite == null)
            {
                defaultSprite = CreateDefaultSprite();
            }
            return defaultSprite;
        }
        return item.icon;
    }
}