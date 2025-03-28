using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Resources;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [SerializeField] private List<QuestItem> allQuests = new List<QuestItem>();
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

        // Inicjalizacja s�ownika dla szybkiego dost�pu
        foreach (var quest in allQuests)
        {
            questDictionary[quest.id] = quest;
        }
    }

    public bool CanStartQuest(string questId)
    {
        if (!questDictionary.TryGetValue(questId, out QuestItem quest))
            return false;

        if (quest.isActive || quest.isCompleted)
            return false;

        // Sprawd� czy wymagane questy s� uko�czone
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

        // Powiadom UI o rozpocz�ciu questa
        UIManager.Instance.ShowQuestStartedNotification(quest.title);

        return true;
    }

    public void UpdateQuestProgress(string targetId, int amount)
    {
        // Aktualizuj post�p wszystkich aktywnych quest�w, kt�re maj� cel o podanym ID
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
        // Sprawd� czy wszystkie cele questa s� uko�czone
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
                    InventoryManager.Instance.AddItem(reward.rewardId, reward.amount);
                    break;
                case QuestReward.RewardType.Skill:
                    PlayerSkills.Instance.AddSkillExperience(reward.rewardId, reward.amount);
                    break;
                case QuestReward.RewardType.Research:
                    ResourceManager.Instance.AddResource("research_points", reward.amount);
                    break;
            }
        }

        // Powiadom UI o uko�czeniu questa
        UIManager.Instance.ShowQuestCompletedNotification(quest.title);
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