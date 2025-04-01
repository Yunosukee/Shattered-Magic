using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    public static PlayerSkills Instance { get; private set; }

    [System.Serializable]
    public class Skill
    {
        public string id;
        public string name;
        public int level;
        public int experience;
        public int experienceToNextLevel;
    }

    [SerializeField] private List<Skill> skills = new List<Skill>();
    private Dictionary<string, Skill> skillDictionary = new Dictionary<string, Skill>();

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

        // Initialize skill dictionary
        foreach (var skill in skills)
        {
            skillDictionary[skill.id] = skill;
        }

        // Add default skills if none exist
        if (skills.Count == 0)
        {
            AddDefaultSkills();
        }
    }

    private void AddDefaultSkills()
    {
        AddSkill("engineering", "Engineering", 1, 0, 100);
        AddSkill("extraction", "Extraction", 1, 0, 100);
        AddSkill("stabilization", "Stabilization", 1, 0, 100);
    }

    private void AddSkill(string id, string name, int level, int exp, int expToNext)
    {
        Skill skill = new Skill
        {
            id = id,
            name = name,
            level = level,
            experience = exp,
            experienceToNextLevel = expToNext
        };

        skills.Add(skill);
        skillDictionary[id] = skill;
    }

    public void AddSkillExperience(string skillId, int amount)
    {
        if (!skillDictionary.ContainsKey(skillId))
        {
            Debug.LogWarning($"[PlayerSkills] Skill {skillId} not found");
            return;
        }

        Skill skill = skillDictionary[skillId];
        skill.experience += amount;

        // Check for level up
        while (skill.experience >= skill.experienceToNextLevel)
        {
            skill.experience -= skill.experienceToNextLevel;
            skill.level++;
            skill.experienceToNextLevel = CalculateExpForNextLevel(skill.level);

            Debug.Log($"[PlayerSkills] {skill.name} leveled up to {skill.level}!");

            // Show level up notification
            if (ToastNotification.Instance != null)
            {
                ToastNotification.Instance.ShowToast($"{skill.name} increased to level {skill.level}!", ToastType.Success);
            }
        }
    }

    private int CalculateExpForNextLevel(int level)
    {
        // Simple formula: 100 * level
        return 100 * level;
    }

    public bool HasSkillLevel(string skillId, int requiredLevel)
    {
        return skillDictionary.ContainsKey(skillId) && skillDictionary[skillId].level >= requiredLevel;
    }

    public int GetSkillLevel(string skillId)
    {
        return skillDictionary.ContainsKey(skillId) ? skillDictionary[skillId].level : 0;
    }
}