using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchItem
{
    public string id;
    public string title;
    public string description;
    public int researchPointsCost;
    public List<ResourceRequirement> resourceRequirements = new List<ResourceRequirement>();
    public List<SkillRequirement> skillRequirements = new List<SkillRequirement>();
    public List<string> effectDescriptions = new List<string>();
    public bool isCompleted = false;
    public Sprite icon;

    // Referencje do innych badañ wymaganych do odblokowania tego
    public List<string> requiredResearchIds = new List<string>();
}

[System.Serializable]
public class ResourceRequirement
{
    public string resourceId;
    public string resourceName;
    public int amount;
}

[System.Serializable]
public class SkillRequirement
{
    public string skillId;
    public string skillName;
    public int requiredLevel;
}