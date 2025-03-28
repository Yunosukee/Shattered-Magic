using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestItemUI : MonoBehaviour
{
    [SerializeField] private Image questIcon;
    [SerializeField] private TMP_Text questTitle;
    [SerializeField] private TMP_Text questDescription;
    [SerializeField] private Transform objectivesContainer;
    [SerializeField] private Transform rewardsContainer;
    [SerializeField] private GameObject objectivePrefab;
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private Button acceptButton;

    private QuestItem questData;
    private List<GameObject> objectiveItems = new List<GameObject>();
    private List<GameObject> rewardItems = new List<GameObject>();

    public void SetQuestData(QuestItem quest)
    {
        questData = quest;

        // Set basic information
        if (questIcon != null && quest.icon != null)
            questIcon.sprite = quest.icon;

        questTitle.text = quest.title;
        questDescription.text = quest.description;

        // Clear existing objectives and rewards
        ClearObjectivesAndRewards();

        // Add objectives
        foreach (var objective in quest.objectives)
        {
            GameObject objGO = Instantiate(objectivePrefab, objectivesContainer);
            objectiveItems.Add(objGO);

            TMP_Text objText = objGO.GetComponentInChildren<TMP_Text>();
            if (objText != null)
            {
                if (quest.isActive)
                {
                    objText.text = $"{objective.description}: {objective.currentAmount}/{objective.requiredAmount}";

                    // Add visual indication for completed objectives
                    if (objective.isCompleted)
                    {
                        objText.color = Color.green;
                    }
                }
                else
                {
                    objText.text = objective.description;
                }
            }
        }

        // Add rewards
        foreach (var reward in quest.rewards)
        {
            GameObject rewGO = Instantiate(rewardPrefab, rewardsContainer);
            rewardItems.Add(rewGO);

            Image rewIcon = rewGO.GetComponentInChildren<Image>();
            TMP_Text rewText = rewGO.GetComponentInChildren<TMP_Text>();

            if (rewIcon != null && reward.icon != null)
                rewIcon.sprite = reward.icon;

            if (rewText != null)
            {
                string rewardTypeText = "";
                switch (reward.type)
                {
                    case QuestReward.RewardType.Resource:
                        rewardTypeText = "Resource";
                        break;
                    case QuestReward.RewardType.Item:
                        rewardTypeText = "Item";
                        break;
                    case QuestReward.RewardType.Skill:
                        rewardTypeText = "Experience";
                        break;
                    case QuestReward.RewardType.Research:
                        rewardTypeText = "Research Points";
                        break;
                }

                rewText.text = $"{rewardTypeText}: {reward.rewardName} x{reward.amount}";
            }
        }
    }

    public void SetActiveMode(bool active)
    {
        // Hide accept button for active quests
        if (acceptButton != null)
            acceptButton.gameObject.SetActive(!active);

        // You can add more UI modifications for active quests
    }

    private void ClearObjectivesAndRewards()
    {
        // Clear objectives
        foreach (var item in objectiveItems)
        {
            Destroy(item);
        }
        objectiveItems.Clear();

        // Clear rewards
        foreach (var item in rewardItems)
        {
            Destroy(item);
        }
        rewardItems.Clear();
    }
}