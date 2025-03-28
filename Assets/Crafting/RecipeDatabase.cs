using System.Collections.Generic;
using UnityEngine;

public class RecipeDatabase : ScriptableObject
{
    public List<RecipeData> recipes = new List<RecipeData>();
    private static RecipeDatabase instance;
    public static RecipeDatabase Instance
    {
        get => instance;
        set => instance = value;
    }


    void OnEnable()
    {
        Instance = this;
        Debug.Log($"[RecipeDatabase] Ready - Recipes count: {recipes.Count}");
    }

    public RecipeData GetRecipe(uint recipeId)
    {
        return recipes.Find(x => x.id == recipeId);
    }
}