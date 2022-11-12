using UnityEngine;
using System.Collections.Generic;

namespace Economy
{
    public class ResourceTransferRequestsManager
    {
        private List<ResourcePickUpRequest> _pickupRequests = new();
        private List<ResourceDeliveryRequest> _deliveryRequests = new();

        private List<ResourceTransferRequest> _openRequests = new(); // Open to be fullfilled.
        private List<ResourceTransferRequest> _promisedRequests = new(); // Assigned to be fullfilled.


        /// <summary>Will add to existing request of given storage/type, or create a new request.</summary>
        public void RequestPickup(StorageEntity source, ResourceType resourceType, int amount)
        {
            VerifyRequest(resourceType, amount);

            var request = _pickupRequests.Find(pur => pur.requester == source && pur.resourceStack.type == resourceType);
            if (request == null)
                AddPickupRequest(new ResourcePickUpRequest(source, new(resourceType, amount)));
            else
                request.resourceStack.amount += amount;
        }
        /// <summary>Will add to existing request of given storage/type, or create a new request.</summary>
        public void RequestDelivery(StorageEntity source, ResourceType resourceType, int amount)
        {
            VerifyRequest(resourceType, amount);

            var request = _deliveryRequests.Find(dr => dr.requester == source && dr.resourceStack.type == resourceType);
            if (request == null)
                AddDeliveryRequest(new ResourceDeliveryRequest(source, new(resourceType, amount)));
            else
                request.resourceStack.amount += amount;
        }

        private void VerifyRequest(ResourceType resourceType, int amount)
        {
            Debug.Assert(resourceType != ResourceType.Invalid);
            Debug.Assert(amount != 0);
            if (amount < 0)
                Debug.LogWarningFormat("Requesting {0} resources of type {1}, was this intentional?", amount, resourceType.ToString());
        }
        private void AddPickupRequest(ResourcePickUpRequest request)
        {
            _pickupRequests.Add(request);
            _openRequests.Add(request);
        }
        private void AddDeliveryRequest(ResourceDeliveryRequest request)
        {
            _deliveryRequests.Add(request);
            _openRequests.Add(request);
        }

        public ResourceTransferRequest GetNextRequest(ResourceTransferRequest request) // Can be used in a loop, where the return value is used as input for next call.
        {
            if (_openRequests.Count == 0)
                return null;

            if (request == null)
                return _openRequests[0];

            int i = _openRequests.IndexOf(request);
            if (i == _openRequests.Count - 1) // Last request in list.
                return null;
            else
                return _openRequests[i + 1];
        }

        public ResourceTransferRequest PromiseToFulfill(ResourceTransferRequest request, int amount)
        {
            Debug.LogFormat("Promising to fullfill {0}x{1}", amount, request.resourceStack.type);

            Debug.Assert(amount <= request.resourceStack.amount); // Shouldn't make promises you can't keep.

            if (amount < request.resourceStack.amount)
            {
                // Split request into an open and a promised request.
                request.resourceStack.amount -= amount;

                ResourceTransferRequest promise_request;
                if (request is ResourcePickUpRequest)
                    promise_request = new ResourcePickUpRequest(request.requester, new(request.resourceStack.type, amount));
                else // /(request is ResourceDeliveryRequest)
                    promise_request = new ResourceDeliveryRequest(request.requester, new(request.resourceStack.type, amount));
                _promisedRequests.Add(promise_request);

                return promise_request;
            }
            else if (amount == request.resourceStack.amount)
            {
                // Move to assigned requests.
                _openRequests.Remove(request);
                _promisedRequests.Add(request);
                return request;
            }
            return null; // Shouldn't happen.
        }

        public void FullFillRequest(ResourceTransferRequest request)
        {
            Debug.Assert(!_openRequests.Contains(request));
            Debug.Assert(_promisedRequests.Contains(request));

            _promisedRequests.Remove(request);

            if (request is ResourcePickUpRequest)
                _pickupRequests.Remove(request as ResourcePickUpRequest);
            if (request is ResourceDeliveryRequest)
                _deliveryRequests.Remove(request as ResourceDeliveryRequest);
        }
    }
}
