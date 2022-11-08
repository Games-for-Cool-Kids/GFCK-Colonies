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

        public TransferResourcesTask(Job job, bool oneTime = false) : base(job, oneTime)
        { }

        public override void Tick()
        {
            base.Tick();

            _timeSinceLastTransfer += Time.deltaTime;
            if (_timeSinceLastTransfer >= job.UnitJobComponent.transferSpeed)
            {
                _timeSinceLastTransfer = 0;
                TransferFirstResourceInList();
            }

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

        public override string GetTaskDebugDescription()
        {
            return "I am transferring " + resourcesToTransfer.Count + " resources";
        }
    }
}
