using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [System.Serializable]
    public class GameSaveData
    {
        public Dictionary<uint, int> skillLevels = new Dictionary<uint, int>();
        public Dictionary<uint, float> skillExperience = new Dictionary<uint, float>();
        public Dictionary<uint, int> inventory = new Dictionary<uint, int>();
        public Dictionary<uint, uint> inventorySlots = new Dictionary<uint, uint>();
    }

    void Awake()
    {
        Instance = this;
        Debug.Log("[SaveSystem] Initialized");
    }

    void Start()
    {
        InvokeRepeating("SaveGame", 300f, 300f);
        Debug.Log("[SaveSystem] Auto-save scheduled");
    }

    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData
        {
            skillLevels = SkillSystem.Instance.skillLevels,
            skillExperience = SkillSystem.Instance.skillExperience,
            inventory = InventorySystem.Instance.items,
            inventorySlots = InventorySystem.Instance.slotItems
        };

        string json = JsonUtility.ToJson(saveData);
        string path = Application.persistentDataPath + "/gameData.json";
        File.WriteAllText(path, json);
        Debug.Log("[SaveSystem] Game Saved!");
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/gameData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

            SkillSystem.Instance.skillLevels = saveData.skillLevels;
            SkillSystem.Instance.skillExperience = saveData.skillExperience;
            InventorySystem.Instance.items = saveData.inventory;
            InventorySystem.Instance.slotItems = saveData.inventorySlots;

            if (InventoryUI.Instance != null)
            {
                InventoryUI.Instance.UpdateAllSlots();
            }

            Debug.Log("[SaveSystem] Game Loaded!");
        }
    }
}