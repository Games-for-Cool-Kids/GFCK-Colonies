using UnityEngine;
using Economy;
using UnityEngine.Assertions;

namespace Jobs
{
    public class LookForResourceTransferRequestTask : Task
    {
        private ResourceTransferRequest _requestToFulfill = null;

        public LookForResourceTransferRequestTask(Job job, TaskFlag flags = TaskFlag.None) : base(job, flags)
        { }

        public override void Tick()
        {
            base.Tick();


            var open_request = FindRequestToFulfill();
            if(open_request == null)
            {
                Finish();
                return;
            }

            PromiseToFulfill(open_request);
            CreateTransferTasks();

            Finish();
        }

        private ResourceTransferRequest FindRequestToFulfill()
        {
            var request_manager = PlayerInfo.Instance.ResourceTransferRequestManager;

            for(ResourceTransferRequest request = request_manager.GetNextOpenRequest(null); 
                request != null; 
                request_manager.GetNextOpenRequest(request))
            {
                if(request is ResourcePickUpRequest)
                {
                    return request; // We can fulfill any pick up request.
                }
                else
                {
                    Debug.LogWarning("Delivery requests not yet implemented!");
                }
            }

            return null;
        }

        private void PromiseToFulfill(ResourceTransferRequest request)
        {
            int fullfill_amount = Mathf.Min(job.UnitJobComponent.inventorySize, request.resourceStack.amount);

            var request_tracker = PlayerInfo.Instance.ResourceTransferRequestManager;
            _requestToFulfill = request_tracker.PromiseToFulfill(request, fullfill_amount);

            Debug.LogFormat("Promise to fulfill {0}, for target inventory with {1}", _requestToFulfill, request.requester.inventory);

            Debug.Assert(_requestToFulfill is ResourcePickUpRequest || _requestToFulfill is ResourceDeliveryRequest);
        }

        private void CreateTransferTasks()
        {
            // Move task.
            var move_to_storage_task = new MoveToObjectTask(job, TaskFlag.OneTime);
            move_to_storage_task.TargetObject = _requestToFulfill.requester.gameObject;

            // Transfer task.
            var transfer_type = _requestToFulfill is ResourcePickUpRequest ? TransferType.PickUp : TransferType.Delivery;

            var transfer_resources_task = new TransferResourceRequestTask(job, transfer_type, TaskFlag.OneTime);
            transfer_resources_task.RequestToFulfill = _requestToFulfill;
            transfer_resources_task.targetStorage = _requestToFulfill.requester;

            // Update job.
            job.AddTask(move_to_storage_task);
            job.AddTask(transfer_resources_task);
        }

        public override void Finish()
        {
            _requestToFulfill = null;

            base.Finish();
        }

        public override string GetTaskDescription()
        {
            return "I am looking for resources to pick up or deliver.";
        }
    }
}
