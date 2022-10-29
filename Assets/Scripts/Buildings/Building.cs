using System;
using System.Collections.Generic;
using UnityEngine;
using World;

public class Building : MonoBehaviour
{
    public BuildingGrid buildGrid = new(); // Need to add derived classes to BuildingEditor.

    public Dictionary<ResourceType, int> StoredResources = new Dictionary<ResourceType, int>();

    public event EventHandler<ResourceType> ResourceAdded;

    public List<Job> jobs = new List<Job>();

    private bool _firstUpdate = true;

    public virtual void Start()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            StoredResources.Add(type, 0);
        }

        ShowResourceDisplay(false);
    }

    private void Update()
    {
        // Set grid after first update, because bounding box of renderer is only correct after first render.
        if (_firstUpdate)
        {
            CreateBuildGrid();
            _firstUpdate = false;
        }
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
        foreach (int storedAmount in StoredResources.Values)
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
        foreach (Job job in jobs)
        {
            JobManager.Instance.RegisterJob(job);
        }
    }

    public void AddJob(JobType type)
    {
        var job = JobFactory.CreateJob(type, this);
        jobs.Add(job);
    }

    public Block GetCurrentBlock()
    {
        return GameManager.Instance.World.GetBlockAt(transform.position + Vector3.down / 2);
    }

    private void CreateBuildGrid()
    {
        var bounds = gameObject.GetGridBounds();
        buildGrid.ResizeGrid(Mathf.FloorToInt(bounds.size.x), Mathf.FloorToInt(bounds.size.z));
    }
}
