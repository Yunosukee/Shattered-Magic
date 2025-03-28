using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private GameObject questItemPrefab;
    [SerializeField] private Transform availableQuestsContainer;
    [SerializeField] private Transform activeQuestsContainer;

    private List<GameObject> availableQuestItems = new List<GameObject>();
    private List<GameObject> activeQuestItems = new List<GameObject>();

    private void OnEnable()
    {
        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {
        // Clear existing items
        ClearQuestItems();

        // Get available and active quests
        var availableQuests = QuestManager.Instance.GetAvailableQuests();
        var activeQuests = QuestManager.Instance.GetActiveQuests();

        // Create UI elements for available quests
        foreach (var quest in availableQuests)
        {
            GameObject itemGO = Instantiate(questItemPrefab, availableQuestsContainer);
            availableQuestItems.Add(itemGO);

            // Set quest data in UI
            QuestItemUI itemUI = itemGO.GetComponent<QuestItemUI>();
            itemUI.SetQuestData(quest);

            // Add listener to button
            Button button = itemGO.GetComponent<Button>();
            button.onClick.AddListener(() => OnQuestButtonClicked(quest.id));
        }

        // Create UI elements for active quests
        foreach (var quest in activeQuests)
        {
            GameObject itemGO = Instantiate(questItemPrefab, activeQuestsContainer);
            activeQuestItems.Add(itemGO);

            // Set quest data in UI
            QuestItemUI itemUI = itemGO.GetComponent<QuestItemUI>();
            itemUI.SetQuestData(quest);
            itemUI.SetActiveMode(true); // Special mode for active quests
        }
    }

    private void ClearQuestItems()
    {
        // Clear available quests
        foreach (var item in availableQuestItems)
        {
            Destroy(item);
        }
        availableQuestItems.Clear();

        // Clear active quests
        foreach (var item in activeQuestItems)
        {
            Destroy(item);
        }
        activeQuestItems.Clear();
    }

    private void OnQuestButtonClicked(string questId)
    {
        if (QuestManager.Instance.StartQuest(questId))
        {
            // Quest started successfully, update UI
            UpdateQuestUI();
        }
    }
}