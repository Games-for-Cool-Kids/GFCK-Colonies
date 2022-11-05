using System;
using System.Collections.Generic;

namespace Economy
{
    [Serializable]
    public class ResourceDictionary : UDictionary<ResourceType, int> { }

    public class Inventory
    {
        public ResourceDictionary storedResources = new();

        public delegate void ResourceAmountChangedEvent(ResourceType resource, int changeAmount);
        public ResourceAmountChangedEvent ResourceChanged;

        public Inventory()
        {
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
                storedResources.Add(type, 0);
        }

        public int GetCount(ResourceType resource)
        {
            return storedResources[resource];
        }

        public void SetResource(ResourceType resource, int amount)
        {
            int oldAmount = storedResources[resource];

            storedResources[resource] = amount;
            ResourceChanged?.Invoke(resource, amount - oldAmount);
        }

        public void AddResource(ResourceType resource, int amount = 1)
        {
            storedResources[resource] += amount;
            ResourceChanged?.Invoke(resource, amount);
        }
        public void RemoveResource(ResourceType resource, int amount = 1)
        {
            storedResources[resource] -= amount;
            ResourceChanged?.Invoke(resource, -amount);
        }

        public bool HasResources()
        {
            foreach (int storedAmount in storedResources.Values)
                if (storedAmount > 0)
                    return true;

            return false;
        }

        public override string ToString()
        {
            string s = "";
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            { 
                s += type.ToString();
                s += "-";
                s += GetCount(type);
                s += ", ";
            }
            return s;
        }
    }
}
