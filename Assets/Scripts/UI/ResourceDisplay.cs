using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Economy;

public class ResourceDisplay : MonoBehaviour
{
    private Inventory _linkedInventory;

    [Serializable]
    public class ResourceDictionary : UDictionary<ResourceType, Sprite> { }
    public ResourceDictionary ResourceIcons;

    public GameObject ResourceCountIconPrefab;
    public Dictionary<ResourceType, ResourceCountIcon> ResourceCountIconElements = new Dictionary<ResourceType, ResourceCountIcon>();

    private void Start()
    {
        _linkedInventory = gameObject.GetComponentInParent<StorageEntity>().inventory;
        _linkedInventory.ResourceChanged += UpdateUIVisibility;

        foreach (ResourceType resource in Enum.GetValues(typeof(ResourceType)))
            CreateResourceUIElement(resource);
    }

    private void UpdateUIVisibility(object sender, ResourceType resource)
    {
        SetVisible(_linkedInventory.HasResources());

        int resourceCount = _linkedInventory.GetCount(resource);
        ResourceCountIconElements[resource].setCount(resourceCount);
        ResourceCountIconElements[resource].SetVisible(resourceCount > 0);
    }

    private void CreateResourceUIElement(ResourceType resource)
    {
        GameObject newResourceCountIcon = Instantiate(ResourceCountIconPrefab, transform.GetChild(0)); // Add to first child which is the layout.

        ResourceCountIcon resourceIcon = newResourceCountIcon.GetComponentInChildren<ResourceCountIcon>();
        resourceIcon.SetIcon(ResourceIcons[resource]);
        resourceIcon.SetVisible(false); // Hidden by default, until specific resource is added.

        ResourceCountIconElements.Add(resource, resourceIcon);
    }

    private void SetVisible(bool visible)
    {
        GetComponent<Canvas>().enabled = visible; // Use Canvas to toggle visibility.
    }
}
