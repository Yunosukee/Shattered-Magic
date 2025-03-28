using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ItemInitializer
{
    public static void InitializeAllItems(ItemDatabase database)
    {
        // Za³aduj wszystkie sprite z folderu Resources/Sprites
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>("Sprites");
        // Utwórz s³ownik, aby póŸniej móc wyszukaæ sprite po nazwie
        Dictionary<string, Sprite> spriteDictionary = loadedSprites.ToDictionary(sp => sp.name, sp => sp);
        
        database.items.Add(new ItemData
        {
            id = 1,
            name = "Crystal",
            type = ItemType.Crystal,
            description = "A magical crystal"
        });

        database.items.Add(new ItemData
        {
            id = 2,
            name = "Basic Tool",
            type = ItemType.Tool,
            description = "A basic crafting tool",
            icon = spriteDictionary.ContainsKey("BasicTool") ? spriteDictionary["BasicTool"] : null
        });

        database.items.Add(new ItemData
        {
            id = 3,
            name = "Crystal Tool",
            type = ItemType.Tool,
            description = "A tool infused with crystal magic"
        });

        database.items.Add(new ItemData
        {
            id = 4,
            name = "Stone",
            type = ItemType.Resource,
            description = "Basic building material"
        });

        database.items.Add(new ItemData
        {
            id = 5,
            name = "Softwood",
            type = ItemType.Resource,
            description = "Basic wood material"
        });

        database.items.Add(new ItemData
        {
            id = 6,
            name = "Common Herb",
            type = ItemType.Resource,
            description = "Basic herb material"
        });
        Debug.Log($"ItemInitializer: Added {database.items.Count} items");
    }
}