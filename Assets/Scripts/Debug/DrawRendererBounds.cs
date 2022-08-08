using System;
using UnityEngine;

public class DrawRendererBounds : MonoBehaviour
{
    public bool snapToGrid = false;

    void OnDrawGizmos()
    {
        if (gameObject.TryGetComponent<Renderer>(out var renderer))
        {
            var bounds = renderer.bounds;

            if(snapToGrid)
            {
                bounds = GameObjectUtil.GetGridBounds(gameObject);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}
