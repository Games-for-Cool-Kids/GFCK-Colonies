namespace Jobs
{
    public class TransferAllResourcesTask : TransferResourcesTask
    {
        public TransferAllResourcesTask(Job job, TaskFlag flags = TaskFlag.None) : base(job, flags)
        { }

        public override void Start()
        {
            base.Start();

            // Add all resources in unit inventory.
            foreach(var resource in job.GetAssignedUnit().inventory.storedResources)
                resourcesToTransfer.Add(new(resource.Key, resource.Value));
        }
    }
}
