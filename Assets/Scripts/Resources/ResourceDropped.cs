using UnityEngine;

public class ResourceDropped : MonoBehaviour
{
    public ResourceType ResourceType;

    public void PickUp()
    {
        ResourceManager.Instance.RemoveResourceFromWorld(this);
    }
}
