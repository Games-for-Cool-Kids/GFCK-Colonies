namespace Economy
{
    public class ResourceStack
    {
        public ResourceType type = ResourceType.RESOURCE_INVALID;
        public int amount = 0;

        public ResourceStack(ResourceType type, int amount)
        {
            this.type = type;
            this.amount = amount;
        }
    }
}
