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
        _linkedInventory.ResourceChanged += UpdateResource;
    }

    private void UpdateResource(object sender, ResourceType resource)
    {
        if (!ResourceCountIconElements.ContainsKey(resource)) // Only add if not created yet.
            CreateResourceUIElement(resource);

        ResourceCountIconElements[resource].setCount(_linkedInventory.GetResource(resource));
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
