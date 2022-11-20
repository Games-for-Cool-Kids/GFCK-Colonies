
namespace Economy
{
    // Requests a type of resource to be picked up from an inventory.
    public class ResourcePickUpRequest : ResourceTransferRequest
    {
        public ResourcePickUpRequest(StorageEntity requester, ResourceStack resourceStack) 
            : base(requester, resourceStack)
        { }

        public override string ToString()
        {
            return string.Format("Pickup {0}x{1} request", ResourceStack.amount, ResourceStack.type);
        }
    }
}
