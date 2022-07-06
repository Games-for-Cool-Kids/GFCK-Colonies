using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    RESOURCE_WOOD,
    RESOURCE_STONE,
    RESOURCE_INVALID
}

public class ResourceManager : MonoBehaviour
{
    // TODO We miiiiight not want a singleton 
    private static ResourceManager _instance;
    public static ResourceManager Instance
    {
        get
        {
            Debug.Assert(_instance != null);

            return _instance;
        }
    }

    private List<ResourceDropped> _resourcesInWorldUnclaimed = new List<ResourceDropped>();
    private List<ResourceDropped> _resourcesInWorldClaimed = new List<ResourceDropped>();

    // TODO This works, but is currently not necessary. Bring back if needed, and decide if it should hold claimed/unclaimed/both resources
    // An "inverse" Dictionary, where we can efficiently find a dropped resource in the world based on a given resource type
    //private Dictionary<ResourceType, List<ResourceDropped>> _ResourcesInWorldPerTypeUnclaimed = new Dictionary<ResourceType, List<ResourceDropped>>();

    private void Awake()
    {
        _instance = this;
    }

    public void AddResourceToWorld(ResourceDropped resourceInWorld)
    {
        //ResourceType type = resourceInWorld.ResourceType;

        AddResourceToContainer(resourceInWorld, _resourcesInWorldUnclaimed);

        //_ResourcesInWorldPerTypeUnclaimed[type].Add(resourceInWorld);

        Debug.Log("Keeping track of an instantiated resource. Now tracking: " + _resourcesInWorldUnclaimed.Count + " resources in the world.");
    }

    // Marks a resource as claimed, so it can't be picked up by someone else any more
    public void ClaimResource(ResourceDropped resourceInWorld)
    {
        MoveResourceBetweenContainers(resourceInWorld, _resourcesInWorldUnclaimed, _resourcesInWorldClaimed);
    }

    // Makes a resource, which was previously claimed, available to the world again so it can be claimed by others
    public void UnclaimResource(ResourceDropped resourceInWorld)
    {
        MoveResourceBetweenContainers(resourceInWorld, _resourcesInWorldClaimed, _resourcesInWorldUnclaimed);
    }

    public void RemoveResourceFromWorld(ResourceDropped resourceInWorld)
    {
        if(_resourcesInWorldClaimed.Contains(resourceInWorld))
        {
            RemoveResourceFromContainer(resourceInWorld, _resourcesInWorldClaimed);
        }
        else
        {
            RemoveResourceFromContainer(resourceInWorld, _resourcesInWorldUnclaimed);
        }
    }

    private void MoveResourceBetweenContainers(ResourceDropped resourceInWorld, List<ResourceDropped> from, List<ResourceDropped> to)
    {
        Debug.Assert(from.Contains(resourceInWorld));
        Debug.Assert(!to.Contains(resourceInWorld));

        AddResourceToContainer(resourceInWorld, to);
        RemoveResourceFromContainer(resourceInWorld, from);

        // It's almost as if these could be unit tests instead. In any case, no performance lost in release mode
        Debug.Assert(!from.Contains(resourceInWorld));
        Debug.Assert(to.Contains(resourceInWorld));
    }

    private void AddResourceToContainer(ResourceDropped resourceInWorld, List<ResourceDropped> container)
    {
        Debug.Assert(!container.Contains(resourceInWorld));

        container.Add(resourceInWorld);
    }

    private void RemoveResourceFromContainer(ResourceDropped resourceInWorld, List<ResourceDropped> container)
    {
        Debug.Assert(container.Contains(resourceInWorld));

        // To remove, move last element into the "removed" element
        int elementToBeRemoved = container.IndexOf(resourceInWorld);
        int lastElement = container.Count - 1;

        if (elementToBeRemoved != lastElement)
        {
            container[elementToBeRemoved] = container[lastElement];
        }

        container[lastElement] = null;
    }
}
