using System.Collections.Generic;
using UnityEngine;

public static class RecipeInitializer
{
    public static void InitializeAllRecipes(RecipeDatabase database, ItemDatabase itemDB)
    {
        Debug.Log("[RecipeInitializer] Starting recipe initialization with provided ItemDB");

        database.recipes.Add(new RecipeData
        {
            id = 1,
            name = "Crystal Tool",
            requirements = new List<CraftingRequirement>
            {
                new CraftingRequirement { item = itemDB.GetItem(1), quantity = 2 },
                new CraftingRequirement { item = itemDB.GetItem(2), quantity = 1 }
            },
            resultItem = itemDB.GetItem(3),
            resultQuantity = 1,
            craftingDuration = 5f,
            requiredLevel = 1
        });

        database.recipes.Add(new RecipeData
        {
            id = 2,
            name = "Basic Tool",
            requirements = new List<CraftingRequirement>
            {
                new CraftingRequirement { item = itemDB.GetItem(4), quantity = 2 },
                new CraftingRequirement { item = itemDB.GetItem(5), quantity = 1 }
            },
            resultItem = itemDB.GetItem(2),
            resultQuantity = 1,
            craftingDuration = 5f,
            requiredLevel = 1
        });

        Debug.Log($"[RecipeInitializer] Added {database.recipes.Count} recipes using provided ItemDB");
    }
}