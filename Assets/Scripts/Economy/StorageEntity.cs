using System;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    /// <summary>
    /// Base class for any GameObject that needs an inventory to store resources. Can be a unit, building, etc.
    /// 
    /// Can request resource transfers for pick up and deliver.
    /// </summary>
    public class StorageEntity : MonoBehaviour
    {
        public Inventory inventory = new();

        private List<ResourcePickUpRequest> _resourcePickupRequests = new();
        private List<ResourceDeliveryRequest> _resourceDeliverRequests = new();

        public delegate void PickUpRequestEvent(ResourcePickUpRequest request);
        public PickUpRequestEvent PickUpRequestChanged;
        public delegate void DeliveryRequestEvent(ResourceDeliveryRequest request);
        public DeliveryRequestEvent DeliveryRequestChanged;

        protected virtual void Start()
        {
            PlayerInfo.Instance.resourceTransferRequestTracker.RegisterStorageEntity(this);
        }

        protected virtual void OnDestroy() // Called on destruction.
        {
            PlayerInfo.Instance.resourceTransferRequestTracker.UnregisterStorageEntity(this);
        }

        /// <summary>Will add to a pick up request of the resource type, or create a new request if it doesn't exist yet.</summary>
        public void AddToPickupRequest(ResourceType resourceType, int amount)
        {
            Debug.Assert(amount != 0);
            if (amount < 0)
                Debug.LogWarningFormat("Request {0} resources of type {1}, was this intentional?", amount, resourceType.ToString());
            Debug.Assert(resourceType != ResourceType.RESOURCE_INVALID);

            var request = GetOrCreatePickupRequestOfType(resourceType);
            request.resourceStack.amount += amount;

            Debug.LogFormat("Requesting pickup of {0} x {1}", request.resourceStack.amount, request.resourceStack.type);

            PickUpRequestChanged?.Invoke(request);
        }
        /// <summary>Will add to a delivery request of the resource type, or create a new request if it doesn't exist yet.</summary>
        public void AddToDeliveryRequest(ResourceType resourceType, int amount)
        {
            Debug.Assert(amount != 0);
            if (amount < 0)
                Debug.LogWarningFormat("Request {0} resources of type {1}, was this intentional?", amount, resourceType.ToString());
            Debug.Assert(resourceType != ResourceType.RESOURCE_INVALID);


            var request = GetOrCreateDeliveryRequestOfType(resourceType);
            request.resourceStack.amount += amount;

            Debug.LogFormat("Requesting delivery of {0} x {1}", request.resourceStack.amount, request.resourceStack.type);

            DeliveryRequestChanged?.Invoke(request);
        }

        private ResourcePickUpRequest GetOrCreatePickupRequestOfType(ResourceType resourceType)
        {
            var request_of_type = _resourcePickupRequests.Find(rtr => rtr.resourceStack.type == resourceType);
            if (request_of_type == null) // Create new request for resource. 
            {
                request_of_type = new ResourcePickUpRequest(this, new(resourceType, 0));
                _resourcePickupRequests.Add(request_of_type);
            }
            return request_of_type;
        }
        private ResourceDeliveryRequest GetOrCreateDeliveryRequestOfType(ResourceType resourceType)
        {
            var request_of_type = _resourceDeliverRequests.Find(rtr => rtr.resourceStack.type == resourceType);
            if (request_of_type == null) // Create new request for resource. 
            {
                request_of_type = new ResourceDeliveryRequest(this, new(resourceType, 0));
                _resourceDeliverRequests.Add(request_of_type);
            }
            return request_of_type;
        }
    }
}