using Economy;
using Jobs;

public class ResourceHarvestBuilding : Building
{
    public ResourceType resourceTypeToHarvest { get; private set; }

    public ResourceHarvestBuilding(ResourceType resourceType)
    {
        this.resourceTypeToHarvest = resourceType;
    }

    protected virtual void Start()
    {
        inventory.ResourceChanged += OnResourceAdded;
    }

    private void OnResourceAdded(ResourceType type, int changedAmount)
    {
        if (type == resourceTypeToHarvest)
        {
            AddToHarvestPickupRequest(changedAmount); // By default all resources of harvest type should be picked up.
        }
    }

    private void AddToHarvestPickupRequest(int amount)
    {
        AddToPickupRequest(resourceTypeToHarvest, amount);
    }
}
