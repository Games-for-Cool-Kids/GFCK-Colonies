using System;
using UnityEngine;

public class RemoveBuildingHand : InputResolverStep
{
    public static event EventHandler BuildingRemoved;

    public override bool ResolveInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Collider hitCollider = CameraUtil.CastMouseRayFromCamera().collider;
            if (hitCollider != null
             && hitCollider.CompareTag(GlobalDefines.buildingTag))
            {
                Destroy(hitCollider.gameObject);
                BuildingRemoved.Invoke(this, null);
            }
        }

        return true;
    }
}
