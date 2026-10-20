using UnityEngine;
using System.Collections.Generic;

public enum ResourceType
{
    Wood,
    Stone,
    Rope
}

public class Inventory : MonoBehaviour
{
    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    void Awake()
    {
        resources[ResourceType.Wood] = 0;
        resources[ResourceType.Stone] = 0;
        resources[ResourceType.Rope] = 0;
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (resources.ContainsKey(type))
        {
            resources[type] += amount;
            Debug.Log($"<color=green>[Inventaire] +{amount} {type} ! Total actuel : {resources[type]}</color>");
        }
    }

    public int GetResourceCount(ResourceType type)
    {
        return resources.ContainsKey(type) ? resources[type] : 0;
    }
}