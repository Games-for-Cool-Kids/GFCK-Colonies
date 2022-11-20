
namespace Economy
{
    // Requests a type of resource to be delivered to an inventory.
    public class ResourceDeliveryRequest : ResourceTransferRequest
    {
        public ResourceDeliveryRequest(StorageEntity requester, ResourceStack resourceStack) 
            : base(requester, resourceStack)
        { }

        public override string ToString()
        {
            return string.Format("Delivery {0}x{1} request", ResourceStack.amount, ResourceStack.type);
        }
    }
}
