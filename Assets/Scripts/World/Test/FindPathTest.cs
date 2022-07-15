using System.Collections.Generic;
using UnityEngine;

public class FindPathTest : MonoBehaviour
{
    private World _world;

    private Block _pathStartBlock = null;
    private Block _pathEndBlock = null;

    private void Start()
    {
        _world = GetComponent<World>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                Block hitBlock = _world.GetBlockFromRayHit(hit);
                if (_pathStartBlock == null)
                    _pathStartBlock = hitBlock;
                else if (_pathEndBlock == null)
                    _pathEndBlock = hitBlock;

                if (_pathStartBlock != null && _pathEndBlock != null)
                {
                    Pathfinding.PathfindMaster.GetInstance().RequestPathfind(_world, _pathStartBlock, _pathEndBlock, ShowPath);
                    _pathStartBlock = null;
                    _pathEndBlock = null;
                }
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
