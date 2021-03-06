using System;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Dictionary<ResourceType, int> StoredResources = new Dictionary<ResourceType, int>();

    public event EventHandler<ResourceType> ResourceAdded;

    public List<Job> jobs = new List<Job>();

    public virtual void Start()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            StoredResources.Add(type, 0);
        }

        ShowResourceDisplay(false);
    }

    // Can be done by player-hand, or by villager
    public void DropOffResource(Resource resource)
    {
        ShowResourceDisplay(true);

        StoredResources[resource.Type] += 1;

        ResourceAdded.Invoke(this, resource.Type);

        ResourceManager.Instance.RemoveResourceFromWorld(resource); // Call last since it also destroys the object.
    }

    public bool HasResources()
    {
        foreach(int storedAmount in StoredResources.Values)
        {
            if (storedAmount > 0)
                return true;
        }

        return false;
    }

    private void ShowResourceDisplay(bool show)
    {
        gameObject.GetComponentInChildren<Canvas>().enabled = show; // First child canvas component can be used to show/hide display.
    }

    public void RegisterJobs()
    {
        foreach(Job job in jobs)
        {
            JobManager.Instance.RegisterJob(job);
        }
    }

    public void AddJob(JobType type)
    {
        var job = JobFactory.CreateJob(type, this);
        jobs.Add(job);
    }
}
