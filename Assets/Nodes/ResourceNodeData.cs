using UnityEngine;

[System.Serializable]
public class ResourceNodeData
{
    public uint id;
    public string name;
    public Sprite icon;
    public int requiredLevel;
    public uint requiredSkillId;
    public float gatherTime;
    public bool isMagical;
    public uint outputItemId;
}