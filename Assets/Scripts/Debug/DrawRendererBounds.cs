using System;
using UnityEngine;

public class DrawRendererBounds : MonoBehaviour
{
    void OnDrawGizmos()
    {
        if (gameObject.TryGetComponent<Renderer>(out var renderer))
        {
            var bounds = renderer.bounds;

            var gridBounds = gameObject.GetGridBounds();

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(gridBounds.center, gridBounds.size);
        }
    }
}
