using System;
using UnityEngine;

public class RemoveBuildingHand : MonoBehaviour
{
    public static event EventHandler BuildingRemoved;

    void Update()
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
    }
}
