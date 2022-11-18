using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    /// <summary>
    /// Base class for any GameObject that needs an inventory to store resources. Can be a unit, building, etc.
    /// </summary>
    public class StorageEntity : MonoBehaviour
    {
        public Inventory inventory = new();

        private ResourceDictionary _wantedResources = new ();

        protected void SetRequestedResource(ResourceType type, int amount)
        {
            _wantedResources[type] = amount;
        }

        public Dictionary<ResourceType, int> GetWantedResources()
        {
            var wantedResources = new Dictionary<ResourceType, int>();
            foreach (var resource in _wantedResources)
            {
                var type = resource.Key;
                int wanted = resource.Value;
                int stored = inventory.storedResources.ContainsKey(type)
                    ? inventory.storedResources[type] : 0;

                wantedResources.Add(resource.Key, wanted - stored);
            }

            return wantedResources;
        }

        public Dictionary<ResourceType, int> GetExcessResources()
        {
            var excessResources = new Dictionary<ResourceType, int>();
            foreach (var resource in inventory.storedResources)
            {
                var type = resource.Key;
                int stored = resource.Value;

                if (_wantedResources.ContainsKey(type))
                {
                    int wanted = _wantedResources[type];
                    if (stored > wanted)
                    {
                        excessResources.Add(type, stored - wanted);
                    }
                }
                else
                {
                    excessResources.Add(resource.Key, stored);
                }
            }

            return excessResources;
        }

        /// <summary>Will ask to pick up all non-wanted resources.</summary>
        protected void RequestPickupAll()
        {
            PlayerInfo.Instance.ResourceTransferRequestManager.UpdateRequests(this);
        }
    }
}