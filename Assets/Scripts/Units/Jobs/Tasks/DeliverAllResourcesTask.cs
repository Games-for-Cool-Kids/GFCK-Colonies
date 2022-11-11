namespace Jobs
{
    public class DeliverAllResourcesTask : TransferResourcesTask
    {
        public DeliverAllResourcesTask(Job job,
            TaskFlag flags = TaskFlag.None)
                : base(job, TransferType.Delivery, flags)
        { }

        public override void Start()
        {
            // Add all resources in unit inventory.
            foreach (var resource in job.GetAssignedUnit().inventory.storedResources)
                if (resource.Value > 0)
                    resourcesToTransfer.Add(new(resource.Key, resource.Value));

            base.Start();
        }
    }
}
