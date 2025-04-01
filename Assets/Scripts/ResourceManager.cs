using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private Dictionary<string, int> resources = new Dictionary<string, int>();

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

        // Initialize with some default resources
        resources["research_points"] = 0;
        resources["wood"] = 0;
        resources["stone"] = 0;
    }

    public void AddResource(string resourceId, int amount)
    {
        if (!resources.ContainsKey(resourceId))
        {
            resources[resourceId] = 0;
        }

        resources[resourceId] += amount;

        // Notify UI to update
        Debug.Log($"[ResourceManager] Added {amount} {resourceId}. New total: {resources[resourceId]}");
    }

    public void ConsumeResource(string resourceId, int amount)
    {
        if (!resources.ContainsKey(resourceId) || resources[resourceId] < amount)
        {
            Debug.LogWarning($"[ResourceManager] Not enough {resourceId} to consume {amount}");
            return;
        }

        resources[resourceId] -= amount;
        Debug.Log($"[ResourceManager] Consumed {amount} {resourceId}. Remaining: {resources[resourceId]}");
    }

    public bool HasEnoughResource(string resourceId, int amount)
    {
        return resources.ContainsKey(resourceId) && resources[resourceId] >= amount;
    }

    public int GetResourceAmount(string resourceId)
    {
        return resources.ContainsKey(resourceId) ? resources[resourceId] : 0;
    }
}