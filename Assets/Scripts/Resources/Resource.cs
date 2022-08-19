using UnityEngine;
using System.Collections.Generic;

public enum ResourceType
{
    RESOURCE_INVALID = -1,
    RESOURCE_WOOD = 0,
    RESOURCE_STONE = 1
}

public class Resource : MonoBehaviour
{
    public static Dictionary<ResourceType, string> ResourceTypeToResourceNodeMap = new Dictionary<ResourceType, string>()
    {
        { ResourceType.RESOURCE_WOOD, GlobalDefines.treeResourceNodeName },
        { ResourceType.RESOURCE_STONE, GlobalDefines.stoneResourceNodeName },
        { ResourceType.RESOURCE_INVALID, "INVALID RESOURCE" },
    };

    public ResourceType Type;

    public void PickUp()
    {
        ResourceManager.Instance.RemoveResourceFromWorld(this);
    }
}
