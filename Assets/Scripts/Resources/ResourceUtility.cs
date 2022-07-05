using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    RESOURCE_WOOD,
    RESOURCE_STONE,
    RESOURCE_INVALID
}

[System.Serializable]
public class ResourceGameObjectPair
{
    public ResourceType ResourceType;
    public GameObject VisualPrefab;
}

public class ResourceUtility : MonoBehaviour
{
    // TODO We miiiiight not want a singleton 
    private static ResourceUtility _instance;

    public static ResourceUtility Instance
    {
        get
        {
            Debug.Assert(_instance != null);

            return _instance;
        }
    }

    public List<ResourceGameObjectPair> ResourceGameObjectMapping = new List<ResourceGameObjectPair>();

    private Dictionary<ResourceType, GameObject> _resourceGameObjectMapping = new Dictionary<ResourceType, GameObject>();

    // Start is called before the first frame update
    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        foreach(var pair in ResourceGameObjectMapping)
        {
            _resourceGameObjectMapping[pair.ResourceType] = pair.VisualPrefab;
        }
    }

    public GameObject GetPrefabForResourceType(ResourceType resourceType)
    {
        Debug.Assert(_resourceGameObjectMapping.ContainsKey(resourceType));

        return _resourceGameObjectMapping[resourceType];
    }

}
