using System.Collections.Generic;
using UnityEngine;

public class ResourceNodeDatabase : ScriptableObject
{
    public List<ResourceNodeData> nodes = new List<ResourceNodeData>();
    private static ResourceNodeDatabase instance;
    public static ResourceNodeDatabase Instance
    {
        get => instance;
        set => instance = value;
    }


    void OnEnable()
    {
        Instance = this;
        Debug.Log($"[NodeDatabase] Ready - Nodes count: {nodes.Count}");
    }


    public ResourceNodeData GetNodeData(uint nodeId)
    {
        var node = nodes.Find(x => x.id == nodeId);
        if (node == null)
        {
            Debug.LogWarning($"[NodeDatabase] Node with ID {nodeId} not found!");
        }
        return node;
    }
}