using System;
using System.Collections.Generic;

namespace Economy
{
    public class Inventory
    {
        [Serializable]
        public class ResourceDictionary : UDictionary<ResourceType, int> { }
        private ResourceDictionary _storedResources = new();

        public event EventHandler<ResourceType> ResourceChanged;

        public Inventory()
        {
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
                _storedResources.Add(type, 0);
        }

        public int GetResource(ResourceType resource)
        {
            return _storedResources[resource];
        }

        public void SetResource(ResourceType resource, int amount)
        {
            _storedResources[resource] = amount;
            ResourceChanged?.Invoke(this, resource);
        }

        public void AddResource(ResourceType resource, int amount = 1)
        {
            _storedResources[resource] += amount;
            ResourceChanged?.Invoke(this, resource);
        }
        public void RemoveResource(ResourceType resource, int amount = 1)
        {
            _storedResources[resource] -= amount;
            ResourceChanged?.Invoke(this, resource);
        }

        public bool HasResources()
        {
            foreach (int storedAmount in _storedResources.Values)
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
                s += GetResource(type);
                s += ", ";
            }
            return s;
        }
    }
}
