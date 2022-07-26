using UnityEngine;

public class GameObjectUtil
{
    public static Vector3 GetPivotToMeshMinOffset(GameObject gameObject) // Difference between object pivot and mesh bottom in world space.
    {
        float offset = 0;

        if (gameObject.TryGetComponent<Renderer>(out var renderer))
            offset = gameObject.transform.position.y - renderer.bounds.min.y;

        return Vector3.up * offset;
    }

    public static Vector3 GetObjectBottomPosition(GameObject gameObject)
    {
        return gameObject.transform.position - GetPivotToMeshMinOffset(gameObject);
    }
}
