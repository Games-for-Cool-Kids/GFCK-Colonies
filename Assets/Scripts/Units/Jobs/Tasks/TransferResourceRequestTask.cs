
using Economy;
using System.Diagnostics;

namespace Jobs
{
    public class TransferResourceRequestTask : TransferResourcesTask
    {
        public ResourceTransferRequest RequestToFulfill = null;

        public TransferResourceRequestTask(Job job,
            TransferType transferType,
            TaskFlag flags = TaskFlag.None)
                : base(job, transferType, flags)
        { }

        public override void Start()
        {
            Debug.Assert(RequestToFulfill != null);

            Add(RequestToFulfill.resourceStack);

            base.Start();
        }

        public override void Finish()
        {
            // ToDo: Check if actually fulfilled, task can have been finished early.


            var request_tracker = PlayerInfo.Instance.ResourceTransferRequestManager;
            request_tracker.FullFillRequest(RequestToFulfill);

            RequestToFulfill = null;

            base.Finish();
        }
    }
}
