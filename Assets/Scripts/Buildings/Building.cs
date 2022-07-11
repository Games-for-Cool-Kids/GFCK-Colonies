using System;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Dictionary<ResourceType, int> StoredResources = new Dictionary<ResourceType, int>();

    private void Start()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            StoredResources.Add(type, 0);
        }
    }

    // Can be done by player-hand, or by villager
    public void DropOffResource(Resource resource)
    {
        StoredResources[resource.Type] += 1;
        Debug.Log(StoredResources[resource.Type]);

        ResourceManager.Instance.RemoveResourceFromWorld(resource);
    }
}
