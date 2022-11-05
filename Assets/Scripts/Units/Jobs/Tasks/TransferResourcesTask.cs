using Economy;
using System.Collections.Generic;
using UnityEngine;

namespace Jobs
{
    public class TransferResourcesTask : Task
    {
        public StorageEntity targetStorage = null;

        public List<ResourceStack> resourcesToTransfer = new();

        private float _timeSinceLastTransfer = 0;

        public TransferResourcesTask(Job job, TaskFlag flags = TaskFlag.None) : base(job, flags)
        { }

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
            {
                Finish();
            }
        }

        private void TransferFirstResourceInList()
        {
            ResourceType resourceToTransfer = resourcesToTransfer[0].type;
            // Lower resources to transfer.
            resourcesToTransfer[0].amount -= 1;
            if (resourcesToTransfer[0].amount == 0)
                resourcesToTransfer.RemoveAt(0);

            // Take from unit.
            var unit = job.GetAssignedUnit();
            unit.inventory.RemoveResource(resourceToTransfer);

            // Add to target.
            targetStorage.inventory.AddResource(resourceToTransfer);
        }

        public void AddResourceToTransfer(ResourceType resource, int amount)
        {
            resourcesToTransfer.Add(new(resource, amount));
        }
    }
}
