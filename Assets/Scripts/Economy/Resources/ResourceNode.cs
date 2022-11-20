using UnityEngine;
using World;

public class ResourceNode : MonoBehaviour
{
    public GameObject resourceToDrop = null;
    public ResourceType type = ResourceType.Invalid;

    public float spawnOffset = 3; // Units that the resource spawns towards the camera.

    public int harvestHealth = 50; // If more damage than health, give resource. This means multiple units can work on harvesting a resource at the same time.
    public int harvestDamage = 0;

    /// <summary>Harvest returns success once enough harvest damage was dealt.</summary>
    public bool Harvest(int damage)
    {
        harvestDamage += damage;
        if (harvestDamage >= harvestHealth)
        {
            harvestDamage = 0;
            return true; // Harvest complete.
        }

        return false;
    }

    public void SpawnResource()
    {
        var spawnedResource = CreateResourceObject();

        // TODO Instead of just calling this to track, it might be better if the ResourceManager does the actual spawning too, so the visuals can never be out of sync with the stored data
        ResourceManager.Instance.AddResourceToWorld(spawnedResource);
    }

    public Resource CreateResourceObject()
    {
        Vector3 offset = Camera.main.transform.position - transform.position;
        offset = offset.normalized * 3;

        GameObject newResource = GameManager.Instance.InstantiateGameObject(resourceToDrop);
        newResource.transform.position = transform.position + offset;

        var droppedComponent = newResource.GetComponent<Resource>();
        Debug.Assert(droppedComponent);
        return droppedComponent;
    }
    public Block GetBlock()
    {
        Vector3 blockPos = gameObject.GetObjectBottomPosition() - Vector3.up / 2; // Offset with half a block.
        return GameManager.Instance.World.GetSurfaceBlockUnder(blockPos);
    }
}
