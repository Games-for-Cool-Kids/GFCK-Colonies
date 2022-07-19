using UnityEngine;

public class DrawRendererBounds : MonoBehaviour
{
    void OnDrawGizmos()
    {
        if (gameObject.TryGetComponent<Renderer>(out var renderer))
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(renderer.bounds.center, renderer.bounds.size);
        }
    }
}
