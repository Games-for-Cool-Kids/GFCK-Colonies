using Economy;
using System.Collections.Generic;
using UnityEngine;

namespace Jobs
{
    public enum TransferType : short
    {
        PickUp,
        Delivery,
    }

    public class TransferResourcesTask : Task
    {
        public StorageEntity targetStorage = null;

        public List<ResourceStack> resourcesToTransfer = new();

        private float _timeSinceLastTransfer = 0;
        private TransferType _transferType; 

        public TransferResourcesTask(Job job,
            TransferType transferType,
            TaskFlag flags = TaskFlag.None)
                : base(job, flags)
        {
            _transferType = transferType;
        }

        public override void Start()
        {
            base.Start();

            // This can happen when this task is repeated, unit already emptied his inventory.
            if (resourcesToTransfer.Count == 0)
                Finish();
        }

        public override void Tick()
        {
            base.Tick();

            // Check that we are next to the target. 
            float sqr_distance_to_target = job.GetAssignedUnit().gameObject.GetSqrBBDistanceToObject(targetStorage.gameObject);
            if (sqr_distance_to_target > MathUtil.SQRD_DIAG_DIST_BETWEEN_BLOCKS)
            {
                Debug.LogError("Unit is too far to transfer resources. Check if pathfinding or job are configured correctly!");
                Finish();
                return;
            }

            // Transfer resources.
            _timeSinceLastTransfer += Time.deltaTime;
            if (_timeSinceLastTransfer >= job.UnitJobComponent.transferSpeed)
            {
                _timeSinceLastTransfer = 0;
                TransferFirstResourceInList();
            }
            // No more resources to transfer.
            if (resourcesToTransfer.Count == 0)
                Finish();
        }

        private void TransferFirstResourceInList()
        {
            Debug.Assert(resourcesToTransfer[0].amount > 0);

            var resourceType = resourcesToTransfer[0].type;

            // Lower resources to transfer.
            resourcesToTransfer[0].amount -= 1;
            if (resourcesToTransfer[0].amount == 0)
                resourcesToTransfer.RemoveAt(0);

            Inventory source, target;
            if(_transferType == TransferType.PickUp)
            {
                source = targetStorage.inventory;
                target = job.GetAssignedUnit().inventory;
            }
            else //if(_transferType == TransferType.Deliver)
            {
                source = job.GetAssignedUnit().inventory;
                target = targetStorage.inventory;
            }

            source.RemoveResource(resourceType);
            target.AddResource(resourceType);
        }

        public void Add(ResourceStack resourceStack)
        {
            Add(resourceStack.type, resourceStack.amount);
        }

        public void Add(ResourceType resource, int amount)
        {
            Debug.Assert(amount > 0);

            resourcesToTransfer.Add(new(resource, amount));
        }

        public override string GetTaskDescription()
        {
            return "I am transferring " + resourcesToTransfer.Count + " resources";
        }
    }
}
