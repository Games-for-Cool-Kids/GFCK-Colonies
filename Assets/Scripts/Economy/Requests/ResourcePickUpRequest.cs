
namespace Economy
{
    // Requests a type of resource to be picked up from an inventory.
    public class ResourcePickUpRequest : ResourceTransferRequest
    {
        public ResourcePickUpRequest(StorageEntity requester, ResourceStack resourceStack) 
            : base(requester, resourceStack)
        { }
    }
}
