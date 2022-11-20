using Jobs;
using System;
using UnityEngine;
using World;

public static class GameObjectExtensions
{

    /// <summary>Difference between object pivot position and mesh center bottom position.</summary>
    public static Vector3 GetPivotYOffset(this GameObject gameObject)
    {
        float offset = 0;

        if (gameObject.TryGetComponent<Renderer>(out var renderer))
            offset = gameObject.transform.position.y - renderer.bounds.min.y;

        return Vector3.up * offset;
    }

    public static Vector3 GetObjectBottomPosition(this GameObject gameObject)
    {
        return gameObject.transform.position - GetPivotYOffset(gameObject);
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

    /// <summary>Squared distance between bounding box of this GameObject and bounding box of other GameObject.</summary>
    public static float GetSqrBBDistanceToObject(this GameObject gameObject, GameObject other)
    {
        var thisBB = gameObject.GetGridBounds();
        var otherBB = other.GetGridBounds();

        var p1 = thisBB.ClosestPoint(other.transform.position);
        var p2 = otherBB.ClosestPoint(gameObject.transform.position);

        return (p2 - p1).sqrMagnitude;
    }

    /// <summary>The block that is outside of this objects' BB, closest to point.</summary>
    public static Block GetClosestNeighboringSurfaceBlock(this GameObject gameObject, Vector3 point)
    {
        var bounds = gameObject.GetGridBounds();
        var closestPoint = bounds.ClosestPoint(point);

        Vector3 direction = (point - closestPoint).normalized / 2.0f;
        return GameManager.Instance.World.GetSurfaceBlock(closestPoint + direction);
    }
}
