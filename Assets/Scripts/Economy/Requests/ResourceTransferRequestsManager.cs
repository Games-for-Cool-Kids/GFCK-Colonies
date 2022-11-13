using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Economy
{
    public class ResourceTransferRequestsManager
    {
        private List<ResourcePickUpRequest> _pickupRequests = new();
        private List<ResourceDeliveryRequest> _deliveryRequests = new();

        private List<ResourceTransferRequest> _openRequests = new(); // Open to be fullfilled.
        private List<ResourceTransferRequest> _promisedRequests = new(); // Assigned to be fullfilled. Promised requests should only be removed on fulfillment, as this could break unit jobs.


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
        public void UpdateRequests(StorageEntity storage)
        {
            // Should not have more than one open pickup or delivery request of any resource per storage.
            // Since this would mean that it would both request to pickup and deliver the same resource.
            foreach (var type in EnumUtil.GetValues<ResourceType>())
            {
                var open_requests = _openRequests.Where(req => req.resourceStack.type == type);
                Debug.Assert(open_requests.Count() <= 1);
            }

            UpdateDeliveryRequests(storage);
            UpdatePickUpRequests(storage);

            //VerifyRequest(resourceType, amount);
            //
            //var request = _deliveryRequests.Find(dr => dr.requester == source && dr.resourceStack.type == resourceType);
            //if (request == null)
            //    AddDeliveryRequest(new ResourceDeliveryRequest(source, new(resourceType, amount)));
            //else
            //    request.resourceStack.amount += amount;
        }

        public void UpdateDeliveryRequests(StorageEntity storage)
        {
            var delivery_requests = _deliveryRequests.Where(req => req.requester == storage).ToList();

            foreach (var resource in EnumUtil.GetValues<ResourceType>())
            {
                if (resource == ResourceType.Invalid)
                    continue; // Skip

                var promised_deliveries = _promisedRequests.Where(req => req is ResourceDeliveryRequest)
                                                           .Where(req => req.requester == storage)
                                                           .Where(req => req.resourceStack.type == resource);
                int promised = promised_deliveries.Sum(req => req.resourceStack.amount);

                var open_delivery_request = delivery_requests.Find(req => req.resourceStack.type == resource);
                if (storage.GetWantedResources().TryGetValue(resource, out int wanted))
                {
                    int to_deliver = wanted - promised;

                    if (open_delivery_request != null)
                        open_delivery_request.resourceStack.amount = to_deliver;
                    else if(to_deliver > 0)
                        AddRequest(new ResourceDeliveryRequest(storage, new(resource, to_deliver)));
                }
                else // No longer wanted.
                {
                    if (open_delivery_request != null)
                        RemoveRequest(open_delivery_request);
                }
            }
        }

        public void UpdatePickUpRequests(StorageEntity storage)
        {
            var pickup_requests = _pickupRequests.Where(req => req.requester == storage).ToList();

            foreach (var resource in EnumUtil.GetValues<ResourceType>())
            {
                if (resource == ResourceType.Invalid)
                    continue; // Skip

                var promised_pickups = _promisedRequests.Where(req => req is ResourcePickUpRequest)
                                                        .Where(req => req.requester == storage)
                                                        .Where(req => req.resourceStack.type == resource);
                int promised = promised_pickups.Sum(req => req.resourceStack.amount);

                var open_pickup_request = pickup_requests.Find(req => req.resourceStack.type == resource);
                if (storage.GetExcessResources().TryGetValue(resource, out int excess))
                {
                    int to_pickup = excess - promised;

                    if (open_pickup_request != null)
                        open_pickup_request.resourceStack.amount = to_pickup;
                    else if (to_pickup > 0)
                        AddRequest(new ResourceDeliveryRequest(storage, new(resource, to_pickup)));
                }
                else // No longer wanted.
                {
                    if (open_pickup_request != null)
                        RemoveRequest(open_pickup_request);
                }
            }
        }

        public ResourceTransferRequest GetNextOpenRequest(ResourceTransferRequest request) // Can be used in a loop, where the return value is used as input for next call.
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
                {
                    promise_request = new ResourcePickUpRequest(request.requester, new(request.resourceStack.type, amount));
                    _pickupRequests.Add(promise_request as ResourcePickUpRequest);
                }
                else //(request is ResourceDeliveryRequest)
                {
                    promise_request = new ResourceDeliveryRequest(request.requester, new(request.resourceStack.type, amount));
                    _deliveryRequests.Add(promise_request as ResourceDeliveryRequest);
                }
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

        private void VerifyNewRequest(ResourceTransferRequest request)
        {
            Debug.Assert(request.resourceStack.type != ResourceType.Invalid);
            Debug.Assert(request.resourceStack.amount > 0);

            // Can not exist yet.
            Debug.Assert(_openRequests.Find(req => req.resourceStack.type == request.resourceStack.type) == null);
            //if (request is ResourcePickUpRequest)
            //    Debug.Assert(_pickupRequests.Find(req => req.resourceStack.type == request.resourceStack.type) == null);
            //else
            //    Debug.Assert(_deliveryRequests.Find(req => req.resourceStack.type == request.resourceStack.type) == null);
        }

        private void AddRequest(ResourceTransferRequest request)
        {
            VerifyNewRequest(request);

            _openRequests.Add(request);
            if (request is ResourcePickUpRequest)
                _pickupRequests.Add(request as ResourcePickUpRequest);
            else
                _deliveryRequests.Add(request as ResourceDeliveryRequest);
        }
        private void RemoveRequest(ResourceTransferRequest request)
        {
            _openRequests.Remove(request);
            if (request is ResourcePickUpRequest)
                _pickupRequests.Remove(request as ResourcePickUpRequest);
            else
                _deliveryRequests.Remove(request as ResourceDeliveryRequest);
        }
    }
}
