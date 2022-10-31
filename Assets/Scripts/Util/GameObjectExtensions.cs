using System;
using UnityEngine;

public static class GameObjectExtensions
{
    public static Vector3 GetPivotToMeshMinOffset(this GameObject gameObject) // Difference between object pivot and mesh bottom in world space.
    {
        float offset = 0;

        if (gameObject.TryGetComponent<Renderer>(out var renderer))
            offset = gameObject.transform.position.y - renderer.bounds.min.y;

        return Vector3.up * offset;
    }

    public static Vector3 GetObjectBottomPosition(this GameObject gameObject)
    {
        return gameObject.transform.position - GetPivotToMeshMinOffset(gameObject);
    }

    public static Bounds CalculateRecursiveBounds(this GameObject gameObject)
    {
        Bounds combinedBounds = new();
        if (gameObject.TryGetComponent<Renderer>(out var renderer))
            combinedBounds = renderer.bounds;

        foreach (var childRenderer in gameObject.GetComponentsInChildren<Renderer>())
            combinedBounds.Encapsulate(childRenderer.bounds);

        return combinedBounds;
    }

    public static Bounds GetGridBounds(this GameObject gameObject)
    {
        var renderer = gameObject.GetComponent<Renderer>();
        var bounds = renderer.bounds;

        Vector3 snappedMin = Vector3.zero;
        snappedMin.x = MathF.Floor(bounds.min.x);
        snappedMin.y = MathF.Floor(bounds.min.y);
        snappedMin.z = MathF.Floor(bounds.min.z);
        snappedMin += Vector3.one * 0.5f;

        Vector3 snappedMax = Vector3.zero;
        snappedMax.x = MathF.Ceiling(bounds.max.x);
        snappedMax.y = MathF.Ceiling(bounds.max.y);
        snappedMax.z = MathF.Ceiling(bounds.max.z);
        snappedMax -= Vector3.one * 0.5f;

        Bounds gridBounds = new();
        gridBounds.min = snappedMin;
        gridBounds.max = snappedMax;
        return gridBounds;
    }
}