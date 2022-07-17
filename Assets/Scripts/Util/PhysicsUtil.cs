using UnityEngine;

public class PhysicsUtil
{
    public static LayerMask NothingMask = 0;
    public static LayerMask EverythingMask = ~0;

    static public LayerMask GetInverseLayerMaskFromName(string layerName)
    {
        return ~LayerMask.NameToLayer(layerName);
    }
}
