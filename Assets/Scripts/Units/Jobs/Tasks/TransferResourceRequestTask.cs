
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

            Add(RequestToFulfill.ResourceStack);

            base.Start();
        }

        public override void Finish()
        {
            var request_tracker = PlayerInfo.Instance.ResourceTransferRequestManager;
            request_tracker.FullFillRequest(RequestToFulfill);

            RequestToFulfill = null;

            base.Finish();
        }

        protected override void TransferNextResource()
        {
            RequestToFulfill.Amount -= 1;

            base.TransferNextResource();
        }
    }
}
