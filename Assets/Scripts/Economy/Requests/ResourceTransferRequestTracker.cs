
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;

namespace Economy
{
    /// <summary>Keeps track of all existing requests of the storage entities.</summary>
    public class ResourceTransferRequestTracker
    {
        private List<ResourcePickUpRequest> _resourcePickupRequests = new();
        private List<ResourceDeliveryRequest> _resourceDeliverRequests = new();

        public void RegisterStorageEntity(StorageEntity storage)
        {
            storage.PickUpRequestChanged += AddPickupRequest;
            storage.DeliveryRequestChanged += AddDeliverRequest;
        }

        public void UnregisterStorageEntity(StorageEntity storage)
        {
            storage.PickUpRequestChanged -= AddPickupRequest;
            storage.DeliveryRequestChanged -= AddDeliverRequest;
        }

        public void AddPickupRequest(ResourcePickUpRequest request)
        {
            _resourcePickupRequests.Add(request);
        }
         
        public void AddDeliverRequest(ResourceDeliveryRequest request)
        {
            _resourceDeliverRequests.Add(request);
        }
    }
}
