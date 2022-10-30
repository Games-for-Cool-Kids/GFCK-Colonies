
public static class Conversions
{
    public static string ResourceNodeTagForType(ResourceType resourceType)
    {
        switch(resourceType)
        {
            case ResourceType.RESOURCE_WOOD:
                return GlobalDefines.treeResourceNodeName;
            case ResourceType.RESOURCE_STONE:
                return GlobalDefines.stoneResourceNodeName;
            case ResourceType.RESOURCE_INVALID:
            default:
                UnityEngine.Debug.LogError("No node implemented for resource: " + resourceType);
                return "";
        }
    }
}
