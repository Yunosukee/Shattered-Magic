using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestItem
{
    public string id;
    public string title;
    public string description;
    public List<QuestObjective> objectives = new List<QuestObjective>();
    public List<QuestReward> rewards = new List<QuestReward>();
    public bool isCompleted = false;
    public bool isActive = false;
    public Sprite icon;

    // Referencje do innych questów wymaganych do odblokowania tego
    public List<string> requiredQuestIds = new List<string>();
}

[System.Serializable]
public class QuestObjective
{
    public string description;
    public string targetId; // ID zasobu, przedmiotu, itp.
    public int requiredAmount;
    public int currentAmount;
    public bool isCompleted = false;

    public void UpdateProgress(int amount)
    {
        currentAmount += amount;
        if (currentAmount >= requiredAmount)
        {
            currentAmount = requiredAmount;
            isCompleted = true;
        }
    }
}

[System.Serializable]
public class QuestReward
{
    public enum RewardType { Resource, Item, Skill, Research }

    public RewardType type;
    public string rewardId;
    public string rewardName;
    public int amount;
    public Sprite icon;
}