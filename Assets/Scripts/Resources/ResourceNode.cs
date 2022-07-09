using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public GameObject ResourceToDrop = null;

    public float ResourceRefreshTime = 0.5f; // Seconds
    public float SpawnOffset = 3; // Units that the resource spawns towards the camera.
    
    private System.DateTime timeSinceLastResourceSpawn;

    // Start is called before the first frame update
    void Start()
    {
        timeSinceLastResourceSpawn = System.DateTime.Now;
    }

    public void SpawnResource()
    {
        if((System.DateTime.Now - timeSinceLastResourceSpawn).TotalSeconds >= ResourceRefreshTime)
        {
            Vector3 offset = Camera.main.transform.position - transform.position;
            offset = offset.normalized * 3;

            GameObject newResource = Instantiate(ResourceToDrop);
            newResource.transform.position = transform.position + offset;

            var droppedComponent = newResource.GetComponent<Resource>();
            Debug.Assert(droppedComponent);

            // TODO Instead of just calling this to track, it might be better if the ResourceManager does the actual spawning too, so the visuals can never be out of sync with the stored data
            ResourceManager.Instance.AddResourceToWorld(droppedComponent);
        }
    }
}
