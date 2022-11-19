
namespace Economy
{
    // One resource type per request. So create multiple requests for different resources.
    public class ResourceTransferRequest
    {
        public StorageEntity Requester;
        public ResourceStack ResourceStack;

        public ResourceType ResourceType { get => ResourceStack.type; }
        public int Amount { get => ResourceStack.amount; set => ResourceStack.amount = value; }

        public ResourceTransferRequest(StorageEntity requester, ResourceStack resourceStack)
        {
            this.Requester = requester;
            this.ResourceStack = resourceStack;
        }
    }
}
