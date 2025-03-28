using UnityEngine;

public class ResourceGatherer : MonoBehaviour
{
    public static ResourceGatherer Instance { get; private set; }
    public bool isGathering { get; private set; }

    private float gatheringProgress = 0f;
    private float gatheringTime = 3f;
    private ResourceNodeData currentNode;

    void Awake()
    {
        Instance = this;
        Debug.Log("[ResourceGatherer] Initialized");
    }

    void Update()
    {
        if (isGathering)
        {
            gatheringProgress += Time.deltaTime;

            if (gatheringProgress >= gatheringTime)
            {
                CompleteGathering();
            }
        }
    }

    public void StartGathering(uint nodeId)
    {
        if (!isGathering)
        {
            currentNode = ResourceNodeDatabase.Instance.GetNodeData(nodeId);
            if (currentNode == null) return;

            // Check if player has the required skill
            if (!SkillSystem.Instance.HasSkill(currentNode.requiredSkillId))
            {
                ToastNotification.Instance.ShowToast($"You need to unlock this skill first!", ToastType.Error);
                return;
            }

            // Check skill level requirement
            if (SkillSystem.Instance.GetSkillLevel(currentNode.requiredSkillId) < currentNode.requiredLevel)
            {
                ToastNotification.Instance.ShowToast($"Required level {currentNode.requiredLevel} not met!", ToastType.Error);
                return;
            }

            isGathering = true;
            gatheringProgress = 0f;
            gatheringTime = currentNode.gatherTime / (1f + SkillSystem.Instance.GetSkillLevel(currentNode.requiredSkillId) * 0.1f);
        }
    }


    private void CompleteGathering()
    {
        if (currentNode.isMagical)
        {
            int stabilizationLevel = SkillSystem.Instance.GetSkillLevel(SkillIDs.STABILIZATION);
            MagicStability stability = MagicStabilitySkill.Instance.RollStability(stabilizationLevel);
            SkillSystem.Instance.AddExperience(SkillIDs.STABILIZATION, 1f);
        }

        InventorySystem.Instance.AddItem(currentNode.outputItemId, 1);
        ToastNotification.Instance.ShowToast(currentNode.outputItemId, 1, "Item collected!", ToastType.Success);

        SkillSystem.Instance.AddExperience(currentNode.requiredSkillId, 1f);
        Debug.Log($"[ResourceGatherer] Gathering completed for {currentNode.name}");

        gatheringProgress = 0f;
    }

    public float GetGatheringProgress()
    {
        return isGathering ? gatheringProgress / gatheringTime : 0f;
    }

    public void StopGathering()
    {
        isGathering = false;
        gatheringProgress = 0f;
        Debug.Log("[ResourceGatherer] Gathering stopped");
    }
}