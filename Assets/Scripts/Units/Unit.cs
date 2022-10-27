using System;
using UnityEngine;
using World;

public class Unit : MonoBehaviour
{
    public static event Action<GameObject> OnUnitSpawned;
    public static event Action<GameObject> OnUnitDespawned;

    public float moveSpeed = 5;

    private void Start()
    {
        OnUnitSpawned?.Invoke(gameObject);
    }

    private void OnDestroy()
    {
        OnUnitDespawned?.Invoke(gameObject);
    }

    public Block GetCurrentBlock()
    {
        Vector3 blockPos = gameObject.GetObjectBottomPosition() - Vector3.up / 2; // Offset with half a block.
        return GameManager.Instance.World.GetSurfaceBlockUnder(blockPos);
    }
}
