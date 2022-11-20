
public static class Conversions
{
    public static string ResourceNodeTagForType(ResourceType resourceType)
    {
        switch(resourceType)
        {
            case ResourceType.Wood:
                return GlobalDefines.treeResourceNodeName;
            case ResourceType.Stone:
                return GlobalDefines.stoneResourceNodeName;
            case ResourceType.Invalid:
            default:
                UnityEngine.Debug.LogError("No node implemented for resource: " + resourceType);
                return "";
        }
    }
}
