using UnityEngine;

public class Resource : MonoBehaviour
{
    public ResourceType ResourceType;

    public void PickUp()
    {
        ResourceManager.Instance.RemoveResourceFromWorld(this);
    }
}
