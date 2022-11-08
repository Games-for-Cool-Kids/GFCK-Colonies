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
        var debugChild = GameObject.Instantiate<GameObject>(new GameObject(), gameObject.transform);
        var canvas = debugChild.AddComponent<Canvas>();
        canvas.name = "DebugCanvas";
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.transform.localScale *= 0.1f;

        var debugSubChild = GameObject.Instantiate<GameObject>(new GameObject(), canvas.transform);
        var text = debugSubChild.AddComponent<Text>();
        text.name = "DebugText";
        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        text.alignment = TextAnchor.MiddleCenter;

        var uiFollower = text.gameObject.AddComponent<WorldObjectUIFollower>();
        uiFollower.HeightOffset = 5.0f;
    }
#endif

    private void Start()    {        OnUnitSpawned?.Invoke(gameObject);    }

    private void OnDestroy()    {        OnUnitDespawned?.Invoke(gameObject);    }

    public Block GetCurrentBlock()
    {
        Vector3 blockPos = gameObject.GetObjectBottomPosition() - Vector3.up / 2; // Offset with half a block.
        return GameManager.Instance.World.GetSurfaceBlockUnder(blockPos);
    }
}
