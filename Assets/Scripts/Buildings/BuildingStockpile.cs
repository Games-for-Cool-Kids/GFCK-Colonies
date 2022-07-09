using UnityEngine;

// TODO Later on, when we have another building, we'll probably add a base-class or interface for them
public class BuildingStockpile : MonoBehaviour
{
    // Can be done by player-hand, or by villager
    public void DropOffResource(Resource resource)
    {
        ResourceManager.Instance.RemoveResourceFromWorld(resource);
    }
}
