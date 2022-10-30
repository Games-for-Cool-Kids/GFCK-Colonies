using UnityEngine;
using System.Collections.Generic;


public class Resource : MonoBehaviour
{
    public ResourceType type;

    public ResourceType Type;

    public void PickUp()
    {
        ResourceManager.Instance.RemoveResourceFromWorld(this);
    }
}
