using UnityEngine;

public class GameObjectUtil
{
    public static Vector3 GetPivotToMeshMinOffset(GameObject gameObject) // Difference between object pivot and mesh bottom in world space.
    {
        float offset = 0;

        if (gameObject.TryGetComponent<MeshFilter>(out var mesh_filter))
        {
            var mesh = mesh_filter.mesh;
            if (mesh != null)
            {
                offset = -mesh_filter.mesh.bounds.min.y * gameObject.transform.localScale.y;
            }
        }

        return Vector3.up * offset;
    }
}
