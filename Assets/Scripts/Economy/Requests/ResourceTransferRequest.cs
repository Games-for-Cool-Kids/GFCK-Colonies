
namespace Economy
{
    // One resource type per request. So create multiple requests for different resources.
    public class ResourceTransferRequest
    {
        public StorageEntity requester;
        public ResourceStack resourceStack;

        public ResourceTransferRequest(StorageEntity requester, ResourceStack resourceStack)
        {
            this.requester = requester;
            this.resourceStack = resourceStack;
        }
    }
}
