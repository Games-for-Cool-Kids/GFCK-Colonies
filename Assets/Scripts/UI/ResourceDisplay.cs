using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour
{
    private Building _building;

    [Serializable]
    public class ResourceDictionary : UDictionary<ResourceType, Sprite> { }
    public ResourceDictionary ResourceIcons;

    public GameObject ResourceCountIconPrefab;
    public Dictionary<ResourceType, ResourceCountIcon> ResourceCountIconElements = new Dictionary<ResourceType, ResourceCountIcon>();

    private void Start()
    {
        _building = gameObject.GetComponentInParent<Building>();
        _building.ResourceAdded += AddResource;
    }

    private void AddResource(object sender, ResourceType resource)
    {
        if (!ResourceCountIconElements.ContainsKey(resource)) // Only add if not created yet.
            CreateResourceUIElement(resource);

        ResourceCountIconElements[resource].setCount(_building.StoredResources[resource]);
    }

    void CreateResourceUIElement(ResourceType resource)
    {
        Debug.Assert(transform.GetChild(0).GetComponent<HorizontalLayoutGroup>());
        GameObject newResourceCountIcon = Instantiate(ResourceCountIconPrefab, transform.GetChild(0)); // Add to first child which is the layout.

        ResourceCountIcon icon = newResourceCountIcon.GetComponentInChildren<ResourceCountIcon>();
        icon.SetIcon(ResourceIcons[resource]);

        ResourceCountIconElements.Add(resource, icon);
    }
}
