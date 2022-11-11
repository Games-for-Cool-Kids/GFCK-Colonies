using Economy;
using System;
using UnityEngine;
using UnityEngine.UI;
using World;

public class Unit : StorageEntity
{    public static event Action<GameObject> OnUnitSpawned;
    public static event Action<GameObject> OnUnitDespawned;

    public float moveSpeed = 5;

#if DEBUG
    private void Awake()
    {
        var debugChild = new GameObject("DebugCanvas");
        debugChild.transform.parent = gameObject.transform;
        debugChild.transform.localScale *= 0.1f;

        var canvas = debugChild.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var debugSubChild = new GameObject("DebugText");
        debugSubChild.transform.parent = debugChild.transform;

        var text = debugSubChild.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        text.alignment = TextAnchor.MiddleCenter;

        var uiFollower = text.gameObject.AddComponent<WorldObjectUIFollower>();
        uiFollower.HeightOffset = 5.0f;
    }
#endif

    protected virtual void Start()    {        OnUnitSpawned?.Invoke(gameObject);    }

    protected virtual void OnDestroy()    {        OnUnitDespawned?.Invoke(gameObject);    }

    public Block GetCurrentBlock()
    {
        Vector3 blockPos = gameObject.GetObjectBottomPosition() - Vector3.up / 2; // Offset with half a block.
        return GameManager.Instance.World.GetSurfaceBlockUnder(blockPos);
    }
}
