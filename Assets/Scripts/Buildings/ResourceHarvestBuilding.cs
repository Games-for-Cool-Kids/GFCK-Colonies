using Economy;
using Jobs;

public class ResourceHarvestBuilding : Building
{
    public ResourceType resourceTypeToHarvest { get; private set; }

    public ResourceHarvestBuilding(ResourceType resourceType)
    {
        this.resourceTypeToHarvest = resourceType;

        inventory.ResourceChanged += OnResourceAdded;
    }

    private void OnResourceAdded(ResourceType type, int changedAmount)
    {
        RequestPickupAll();
    }
}
