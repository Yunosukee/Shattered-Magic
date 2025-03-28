using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeData
{
    public uint id;
    public string name;
    public Sprite icon;
    public List<CraftingRequirement> requirements;
    public float craftingDuration;
    public int requiredLevel;
    public float stabilityRequirement;
    public List<MagicType> requiredMagicTypes;
    public ItemData resultItem;
    public int resultQuantity;
}

[System.Serializable]
public class CraftingRequirement
{
    public ItemData item;
    public int quantity;
}