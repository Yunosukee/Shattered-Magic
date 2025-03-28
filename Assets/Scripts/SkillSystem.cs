using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : MonoBehaviour
{
    public static SkillSystem Instance { get; private set; }
    public Dictionary<uint, int> skillLevels = new Dictionary<uint, int>();
    public Dictionary<uint, float> skillExperience = new Dictionary<uint, float>();

    void Awake()
    {
        Instance = this;
        InitializeSkills();
        Debug.Log("[SkillSystem] Initialized");
    }

    private void InitializeSkills()
    {
        // Initialize all skills at level 1
        skillLevels[SkillIDs.MINING] = 1;
        skillLevels[SkillIDs.WOODCUTTING] = 1;
        skillLevels[SkillIDs.HERBALISM] = 1;
        skillLevels[SkillIDs.EXTRACTION] = 1;
        skillLevels[SkillIDs.STABILIZATION] = 1;

        Debug.Log("[SkillSystem] Skills initialized with default levels");
    }

    public void AddExperience(uint skillId, float amount)
    {
        if (!skillExperience.ContainsKey(skillId))
        {
            skillExperience[skillId] = 0;
        }

        skillExperience[skillId] += amount;
        CheckLevelUp(skillId);
        Debug.Log($"[SkillSystem] Added {amount} XP to skill {skillId}");
    }

    private void CheckLevelUp(uint skillId)
    {
        float currentXP = skillExperience[skillId];
        int currentLevel = skillLevels[skillId];
        float requiredXP = CalculateRequiredXP(currentLevel);

        if (currentXP >= requiredXP)
        {
            LevelUp(skillId);
        }
    }

    private void LevelUp(uint skillId)
    {
        skillLevels[skillId]++;
        Debug.Log($"[SkillSystem] Skill {skillId} leveled up to {skillLevels[skillId]}!");
    }

    private float CalculateRequiredXP(int level)
    {
        return level * 100f;
    }

    public int GetSkillLevel(uint skillId)
    {
        return skillLevels.ContainsKey(skillId) ? skillLevels[skillId] : 1;
    }

    public bool HasSkill(uint skillId)
    {
        return skillLevels.ContainsKey(skillId);
    }


    public float GetSkillExperience(uint skillId)
    {
        return skillExperience.ContainsKey(skillId) ? skillExperience[skillId] : 0f;
    }
}