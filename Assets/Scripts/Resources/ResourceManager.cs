using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviourSingleton<ResourceManager>
{
    private List<Resource> _resourcesInWorldUnclaimed = new List<Resource>();
    private List<Resource> _resourcesInWorldClaimed = new List<Resource>();

    // TODO This works, but is currently not necessary. Bring back if needed, and decide if it should hold claimed/unclaimed/both resources
    // An "inverse" Dictionary, where we can efficiently find a dropped resource in the world based on a given resource type
    //private Dictionary<Type, List<Resource>> _ResourcesInWorldPerTypeUnclaimed = new Dictionary<Type, List<Resource>>();

    public void AddResourceToWorld(Resource resourceInWorld)
    {
        //Type type = resourceInWorld.Type;

        AddResourceToContainer(resourceInWorld, _resourcesInWorldUnclaimed);

        //_ResourcesInWorldPerTypeUnclaimed[type].Add(resourceInWorld);

        Debug.Log("Keeping track of an instantiated resource. Now tracking: " + _resourcesInWorldUnclaimed.Count + " resources in the world.");
    }

    // Marks a resource as claimed, so it can't be picked up by someone else any more
    public void ClaimResource(Resource resourceInWorld)
    {
        MoveResourceBetweenContainers(resourceInWorld, _resourcesInWorldUnclaimed, _resourcesInWorldClaimed);
    }

    // Makes a resource, which was previously claimed, available to the world again so it can be claimed by others
    public void UnclaimResource(Resource resourceInWorld)
    {
        MoveResourceBetweenContainers(resourceInWorld, _resourcesInWorldClaimed, _resourcesInWorldUnclaimed);
    }

    public void RemoveResourceFromWorld(Resource resourceInWorld)
    {
        if(_resourcesInWorldClaimed.Contains(resourceInWorld))
        {
            RemoveResourceFromContainer(resourceInWorld, _resourcesInWorldClaimed);
        }
        else
        {
            RemoveResourceFromContainer(resourceInWorld, _resourcesInWorldUnclaimed);
        }

        Destroy(resourceInWorld.gameObject);
    }

    private void MoveResourceBetweenContainers(Resource resourceInWorld, List<Resource> from, List<Resource> to)
    {
        Debug.Assert(from.Contains(resourceInWorld));
        Debug.Assert(!to.Contains(resourceInWorld));

        AddResourceToContainer(resourceInWorld, to);
        RemoveResourceFromContainer(resourceInWorld, from);

        // It's almost as if these could be unit tests instead. In any case, no performance lost in release mode
        Debug.Assert(!from.Contains(resourceInWorld));
        Debug.Assert(to.Contains(resourceInWorld));
    }

    private void AddResourceToContainer(Resource resourceInWorld, List<Resource> container)
    {
        Debug.Assert(!container.Contains(resourceInWorld));

        container.Add(resourceInWorld);
    }

    private void RemoveResourceFromContainer(Resource resourceInWorld, List<Resource> container)
    {
        Debug.Assert(container.Contains(resourceInWorld));

        // To remove, move last element into the "removed" element
        int elementToBeRemoved = container.IndexOf(resourceInWorld);
        int lastElement = container.Count - 1;

        if (elementToBeRemoved != lastElement)
        {
            container[elementToBeRemoved] = container[lastElement];
        }

        container.RemoveAt(lastElement);

        Debug.Log("Stopped tracking an instantiated resource. Now tracking: " + _resourcesInWorldUnclaimed.Count + " resources in the world.");
    }
}
