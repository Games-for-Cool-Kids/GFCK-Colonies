using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Economy
{
    public class ResourceTransferRequestsManager
    {
        private List<ResourceTransferRequest> _openRequests = new(); // Open to be taken for fullfilment.
        private List<(Unit unit, ResourceTransferRequest request)> _promisedRequests = new(); // Assigned to be fullfilled. Promised requests should only be removed on fulfillment, as this could break unit jobs.


        public void UpdateRequests(StorageEntity storage)
        {
            // Should not have more than one open pickup or delivery request of any resource per storage.
            // Since this would mean that the storage would be both requesting to pickup and deliver the same resource.
            foreach (var type in EnumUtil.GetValues<ResourceType>())
            {
                var open_requests = _openRequests
                    .Where(req => req.ResourceType == type)
                    .Where(req => req.Requester == storage);
                Debug.Assert(open_requests.Count() <= 1);
            }

            UpdateDeliveryRequests(storage);
            UpdatePickUpRequests(storage);
        }

        public void UpdateDeliveryRequests(StorageEntity storage)
        {
            var delivery_requests = _openRequests
                .Where(req => req is ResourceDeliveryRequest)
                .Where(req => req.Requester == storage)
                .ToList();
            var promised_deliveries = _promisedRequests.Where(prom => prom.request.Requester == storage).ToList();

            foreach (var resource in EnumUtil.GetValues<ResourceType>())
            {
                if (resource == ResourceType.Invalid)
                    continue; // Skip

                int promised = promised_deliveries
                    .Where(prom => prom.request.ResourceType == resource)
                    .Sum(prom => prom.request.Amount);

                var open_delivery_request = delivery_requests.Find(req => req.ResourceType == resource);
                if (storage.GetWantedResources().TryGetValue(resource, out int wanted)
                 && wanted > 0)
                {
                    int to_deliver = wanted - promised;

                    if (open_delivery_request != null)
                        open_delivery_request.Amount = to_deliver;
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
            var pickup_requests = _openRequests
                .Where(req => req is ResourcePickUpRequest)
                .Where(req => req.Requester == storage)
                .ToList();
            var promised_pickups = _promisedRequests.Where(prom => prom.request.Requester == storage).ToList();

            foreach (var resource in EnumUtil.GetValues<ResourceType>())
            {
                if (resource == ResourceType.Invalid)
                    continue; // Skip

                int promised = promised_pickups
                    .Where(prom => prom.request.ResourceType == resource)
                    .Sum(prom => prom.request.Amount);

                var open_pickup_request = pickup_requests.Find(req => req.ResourceType == resource);
                if (storage.GetExcessResources().TryGetValue(resource, out int excess)
                 && excess > 0)
                {
                    int to_pickup = excess - promised;

                    if (open_pickup_request != null)
                        open_pickup_request.Amount = to_pickup;
                    else if (to_pickup > 0)
                        AddRequest(new ResourcePickUpRequest(storage, new(resource, to_pickup)));
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

        public ResourceTransferRequest PromiseToFulfill(Unit unit, ResourceTransferRequest request, int amount)
        {
            if (amount == request.Amount)
            {
                // Move to promised requests.
                _openRequests.Remove(request);
                _promisedRequests.Add(new(unit, request));
                return request;
            }
            else if (amount < request.Amount)
            {
                // Split request into an open and a promised request.
                request.Amount -= amount;

                ResourceTransferRequest promise_request;
                if (request is ResourcePickUpRequest)
                {
                    promise_request = new ResourcePickUpRequest(request.Requester, new(request.ResourceType, amount));
                }
                else //(request is ResourceDeliveryRequest)
                {
                    promise_request = new ResourceDeliveryRequest(request.Requester, new(request.ResourceType, amount));
                }
                _promisedRequests.Add(new(unit, promise_request));

                return promise_request;
            }

            Debug.Assert(amount <= request.Amount); // Shouldn't make promises you can't keep.
            return null; // Shouldn't happen.
        }

        public void FullFillRequest(ResourceTransferRequest request)
        {
            Debug.Assert(!_openRequests.Contains(request));
            var promised_request = _promisedRequests.Find(unit_req => unit_req.request == request);

            _promisedRequests.Remove(promised_request);
        }

        private void AddRequest(ResourceTransferRequest request)
        {
            Debug.Assert(request.ResourceType != ResourceType.Invalid);
            Debug.Assert(request.Amount > 0);
            Debug.Assert(_openRequests.Find(req => req.ResourceType == request.ResourceType && req.Requester == request.Requester) == null);

            _openRequests.Add(request);
        }
        private void RemoveRequest(ResourceTransferRequest request)
        {
            _openRequests.Remove(request);
        }

        /// <summary>Returns clone of the original list, since outside callers should not modify requests.</summary>
        public List<ResourcePickUpRequest> GetPickupRequestsClone()
        {
            return new(_openRequests
                .Where(req => req is ResourcePickUpRequest)
                .Cast<ResourcePickUpRequest>()
                .ToList());
        }
        /// <summary>Returns clone of the original list, since outside callers should not modify requests.</summary>
        public List<ResourceDeliveryRequest> GetDeliveryRequestsClone()
        {
            return new(_openRequests
                .Where(req => req is ResourceDeliveryRequest)
                .Cast<ResourceDeliveryRequest>()
                .ToList());
        }
        /// <summary>Returns clone of the original list, since outside callers should not modify requests.</summary>
        public List<ResourceTransferRequest> GetOpenRequestsClone()
        {
            return new(_openRequests);
        }
        /// <summary>Returns clone of the original list, since outside callers should not modify requests.</summary>
        public List<(Unit unit, ResourceTransferRequest request)> GetPromisedRequestsClone()
        {
            return new(_promisedRequests);
        }

    }
}
