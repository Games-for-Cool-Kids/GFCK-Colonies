using Jobs;
using System;
using UnityEngine;
using World;

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

    /// <summary>Squared distance between bounding box of this GameObject and bounding box of other GameObject.</summary>
    public static float GetSqrBBDistanceToObject(this GameObject gameObject, GameObject other)
    {
        var thisBB = gameObject.GetGridBounds();
        var otherBB = gameObject.GetGridBounds();

        var p1 = thisBB.ClosestPoint(other.transform.position);
        var p2 = otherBB.ClosestPoint(gameObject.transform.position);

        return (p2 - p1).sqrMagnitude;
    }

    /// <summary>The block that is outside of this objects' BB, closest to point.</summary>
    public static Block GetClosestNeighboringBlock(this GameObject gameObject, Vector3 point)
    {
        var bounds = gameObject.GetGridBounds();
        var closestPoint = bounds.ClosestPoint(point);

        Vector3 direction = (point - closestPoint).normalized / 2.0f;
        return GameManager.Instance.World.GetSurfaceBlockUnder(closestPoint + direction);
    }

    public static Block GetRandomBlockWithinBounds(this GameObject gameObject)
    {
        var bounds = gameObject.GetGridBounds();
        var randX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        var randZ = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);
        var topY = bounds.max.y;

        return GameManager.Instance.World.GetSurfaceBlockUnder(new(randX, topY, randZ));
    }

    public static void PositionOnBlock(this GameObject gameObject, Block block)
    {
        gameObject.transform.position = block.GetSurfaceWorldPos() + gameObject.GetPivotToMeshMinOffset();
    }

}
