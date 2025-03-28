using UnityEditor;
using UnityEngine;

public static class NodeInitializer
{
    private static Sprite LoadNodeSprite(string spriteName)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Nodes/NodeAssets/{spriteName}.png");
    }

    public static void InitializeAllNodes(ResourceNodeDatabase database)
    {
        // Stone Node
        database.nodes.Add(new ResourceNodeData
        {
            id = 1,
            name = "Stone Node",
            icon = LoadNodeSprite("stone_node"),
            requiredLevel = 1,
            requiredSkillId = SkillIDs.MINING,
            gatherTime = 3f,
            isMagical = false,
            outputItemId = 4
        });

        // Softwood Tree Node
        database.nodes.Add(new ResourceNodeData
        {
            id = 2,
            name = "Softwood Tree",
            icon = LoadNodeSprite("tree_node"),
            requiredLevel = 1,
            requiredSkillId = SkillIDs.WOODCUTTING,
            gatherTime = 2.5f,
            isMagical = false,
            outputItemId = 5
        });

        // Common Herb Node
        database.nodes.Add(new ResourceNodeData
        {
            id = 3,
            name = "Common Herb",
            icon = LoadNodeSprite("herb_node"),
            requiredLevel = 1,
            requiredSkillId = SkillIDs.HERBALISM,
            gatherTime = 2f,
            isMagical = false,
            outputItemId = 6
        });

        // Magic Crystal Node
        database.nodes.Add(new ResourceNodeData
        {
            id = 4,
            name = "Magic Crystal",
            icon = LoadNodeSprite("crystal_node"),
            requiredLevel = 1,
            requiredSkillId = SkillIDs.EXTRACTION,
            gatherTime = 3f,
            isMagical = true,
            outputItemId = 1
        });

        Debug.Log($"NodeInitializer: Added {database.nodes.Count} nodes");
    }
}