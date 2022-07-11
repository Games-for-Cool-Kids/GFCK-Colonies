using UnityEngine;

public enum ResourceType
{
    RESOURCE_INVALID = -1,
    RESOURCE_WOOD = 0,
    RESOURCE_STONE = 1
}

public class Resource : MonoBehaviour
{
    public ResourceType Type;

    public void PickUp()
    {
        ResourceManager.Instance.RemoveResourceFromWorld(this);
    }
}
