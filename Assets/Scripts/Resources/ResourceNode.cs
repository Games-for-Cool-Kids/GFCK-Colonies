using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public GameObject Resource = null;

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

            GameObject newResource = Instantiate(Resource);
            newResource.transform.position = transform.position + offset;
        }
    }
}
