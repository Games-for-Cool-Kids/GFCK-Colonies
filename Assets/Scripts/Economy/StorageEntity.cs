using UnityEngine;

namespace Economy
{
    /// <summary>
    /// Base class for any GameObject that needs an inventory to store resources. Can be a unit, building, etc.
    /// </summary>
    public class StorageEntity : MonoBehaviour
    {
        public Inventory inventory = new();
    }
}
