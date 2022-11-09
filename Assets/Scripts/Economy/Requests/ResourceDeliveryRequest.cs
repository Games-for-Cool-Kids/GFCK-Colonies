
namespace Economy
{
    // Requests a type of resource to be delivered to an inventory.
    public class ResourceDeliveryRequest : ResourceTransferRequest
    {
        public ResourceDeliveryRequest(StorageEntity requester, ResourceStack resourceStack) 
            : base(requester, resourceStack)
        { }
    }
}
