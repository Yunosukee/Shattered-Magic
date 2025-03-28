using UnityEngine;

[System.Serializable]
public class ItemData
{
    public uint id;
    public string name;
    public Sprite icon;
    public ItemType type;
    public string description;
}

public enum ItemType
{
    Resource,
    Tool,
    Crystal,
    Consumable
}