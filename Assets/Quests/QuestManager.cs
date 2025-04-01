using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [SerializeField] private List<QuestItem> allQuests = new List<QuestItem>();
    [SerializeField] private ItemDatabase itemDatabase;
    private Dictionary<string, QuestItem> questDictionary = new Dictionary<string, QuestItem>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize dictionary for quick access
        foreach (var quest in allQuests)
        {
            questDictionary[quest.id] = quest;
        }

        Debug.Log("[QuestManager] Initialized with " + allQuests.Count + " quests");
    }

    public bool CanStartQuest(string questId)
    {
        if (!questDictionary.TryGetValue(questId, out QuestItem quest))
            return false;

        if (quest.isActive || quest.isCompleted)
            return false;

        // SprawdŸ czy wymagane questy s¹ ukoñczone
        foreach (var requiredId in quest.requiredQuestIds)
        {
            if (!questDictionary.TryGetValue(requiredId, out QuestItem requiredQuest) || !requiredQuest.isCompleted)
                return false;
        }

        return true;
    }

    public bool StartQuest(string questId)
    {
        if (!CanStartQuest(questId))
            return false;

        QuestItem quest = questDictionary[questId];
        quest.isActive = true;

        // Powiadom UI o rozpoczêciu questa
        ToastNotification.Instance.ShowToast($"Quest started: {quest.title}", ToastType.Info);

        return true;
    }

    public void UpdateQuestProgress(string targetId, int amount)
    {
        // Aktualizuj postêp wszystkich aktywnych questów, które maj¹ cel o podanym ID
        foreach (var quest in allQuests.Where(q => q.isActive && !q.isCompleted))
        {
            bool updated = false;

            foreach (var objective in quest.objectives.Where(o => !o.isCompleted && o.targetId == targetId))
            {
                objective.UpdateProgress(amount);
                updated = true;
            }

            if (updated)
            {
                CheckQuestCompletion(quest);
            }
        }
    }

    private void CheckQuestCompletion(QuestItem quest)
    {
        // SprawdŸ czy wszystkie cele questa s¹ ukoñczone
        if (quest.objectives.All(o => o.isCompleted))
        {
            CompleteQuest(quest);
        }
    }

    private void CompleteQuest(QuestItem quest)
    {
        quest.isCompleted = true;
        quest.isActive = false;

        // Przyznaj nagrody
        foreach (var reward in quest.rewards)
        {
            switch (reward.type)
            {
                case QuestReward.RewardType.Resource:
                    ResourceManager.Instance.AddResource(reward.rewardId, reward.amount);
                    break;
                case QuestReward.RewardType.Item:
                    uint itemId = uint.Parse(reward.rewardId);
                    InventorySystem.Instance.AddItem(itemId, reward.amount);
                    break;
                case QuestReward.RewardType.Skill:
                    PlayerSkills.Instance.AddSkillExperience(reward.rewardId, reward.amount);
                    break;
                case QuestReward.RewardType.Research:
                    ResourceManager.Instance.AddResource("research_points", reward.amount);
                    break;
            }
        }

        // Powiadom UI o ukoñczeniu questa
        ToastNotification.Instance.ShowToast($"Quest completed: {quest.title}", ToastType.Success);
    }

    public List<QuestItem> GetAvailableQuests()
    {
        return allQuests.Where(q => !q.isActive && !q.isCompleted && CanStartQuest(q.id)).ToList();
    }

    public List<QuestItem> GetActiveQuests()
    {
        return allQuests.Where(q => q.isActive && !q.isCompleted).ToList();
    }
}