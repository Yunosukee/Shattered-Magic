using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance { get; private set; }
    private Dictionary<uint, float> craftingProgress = new Dictionary<uint, float>();
    private Dictionary<uint, bool> activeCrafting = new Dictionary<uint, bool>();

    void Awake()
    {
        Instance = this;
        Debug.Log("[CraftingSystem] Initialized");
    }

    void Update()
    {
        foreach (var kvp in activeCrafting.ToList())
        {
            if (kvp.Value)
            {
                UpdateCrafting(kvp.Key);
            }
        }
    }

    public bool StartCrafting(uint recipeId)
    {
        var recipe = RecipeDatabase.Instance.GetRecipe(recipeId);
        if (!CanCraft(recipe))
        {
            Debug.Log($"[CraftingSystem] Cannot craft recipe {recipeId} - missing materials");
            ToastNotification.Instance.ShowToast("Not enough materials!", ToastType.Error);
            return false;
        }

        foreach (var requirement in recipe.requirements)
        {
            InventorySystem.Instance.RemoveItems(requirement.item.id, requirement.quantity);
        }

        activeCrafting[recipeId] = true;
        craftingProgress[recipeId] = 0f;
        Debug.Log($"[CraftingSystem] Started crafting recipe {recipeId}");
        return true;
    }

    public void CancelCrafting(uint recipeId)
    {
        if (activeCrafting.ContainsKey(recipeId) && activeCrafting[recipeId])
        {
            var recipe = RecipeDatabase.Instance.GetRecipe(recipeId);
            foreach (var requirement in recipe.requirements)
            {
                InventorySystem.Instance.AddItem(requirement.item.id, requirement.quantity);
            }

            activeCrafting[recipeId] = false;
            craftingProgress[recipeId] = 0f;
            Debug.Log($"[CraftingSystem] Cancelled crafting recipe {recipeId}");
        }
    }

    private void UpdateCrafting(uint recipeId)
    {
        var recipe = RecipeDatabase.Instance.GetRecipe(recipeId);
        craftingProgress[recipeId] += Time.deltaTime;

        if (craftingProgress[recipeId] >= recipe.craftingDuration)
        {
            CompleteCrafting(recipe);
            activeCrafting[recipeId] = false;
            craftingProgress[recipeId] = 0f;
        }
    }

    private bool CanCraft(RecipeData recipe)
    {
        foreach (var requirement in recipe.requirements)
        {
            if (!InventorySystem.Instance.HasEnough(requirement.item.id, requirement.quantity))
                return false;
        }
        return true;
    }

    private void CompleteCrafting(RecipeData recipe)
    {
        InventorySystem.Instance.AddItem(recipe.resultItem.id, recipe.resultQuantity);
        ToastNotification.Instance.ShowToast(recipe.resultItem.id, recipe.resultQuantity, $"Crafted {recipe.name}!", ToastType.Success);

        // Reset crafting state
        activeCrafting[recipe.id] = false;
        craftingProgress[recipe.id] = 0f;
        CraftingUI.Instance.ResetCraftingState();
    }

    public float GetProgress(uint recipeId)
    {
        if (!craftingProgress.ContainsKey(recipeId))
            return 0f;

        var recipe = RecipeDatabase.Instance.GetRecipe(recipeId);
        return craftingProgress[recipeId] / recipe.craftingDuration;
    }


    public bool IsCrafting(uint recipeId)
    {
        return activeCrafting.ContainsKey(recipeId) && activeCrafting[recipeId];
    }
}