using System.Collections.Generic;
using UnityEngine;
using World;

public class FindPathTest : MonoBehaviour
{
    private GameWorld _world;

    private Block _pathStartBlock = null;
    private Block _pathEndBlock = null;

    private void Start()
    {
        _world = GetComponent<GameWorld>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var block = _world.GetBlockUnderMouse();
            if (block == null)
                return;

            if (_pathStartBlock == null)
                _pathStartBlock = block;
            else if (_pathEndBlock == null)
                _pathEndBlock = block;

            if (_pathStartBlock != null && _pathEndBlock != null)
            {
                Pathfinding.PathfindMaster.Instance.RequestPathfind(_pathStartBlock, _pathEndBlock, ShowPath);
                _pathStartBlock = null;
                _pathEndBlock = null;
            }
        }
    }
    void ShowPath(List<Block> path)
    {
        if (path.Count == 0)
        {
            Debug.Log("No path could be found.");
            return;
        }

        var line = new GameObject("Path");
        line.transform.parent = transform;

        var lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.positionCount = path.Count;
        lineRenderer.widthMultiplier = 0.2f;
        for (int i = 0; i < path.Count; i++)
        {
            lineRenderer.SetPosition(i, path[i].worldPosition + Vector3.up);
        }
    }
}
