using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

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

    public BlockData GetCurrentBlock()
    {
        Vector3 blockPos = GameObjectUtil.GetObjectBottomPosition(gameObject) - Vector3.up / 2; // Offset with half a block.
        return GameManager.Instance.World.GetSurfaceBlockUnder(blockPos);
    }
}
