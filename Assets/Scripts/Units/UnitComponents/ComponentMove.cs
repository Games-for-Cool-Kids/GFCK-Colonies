using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class ComponentMove : BaseUnitComponent
{
    private List<BlockData> _path = null;
    private int _pathIndex = 0;
    private LineRenderer _pathVisualization = null;

    private Unit _unit;

    public bool lookingForPath { get; private set; } = false;
  

    // Start is called before the first frame update
    void Start()
    {
        _unit = GetComponent<Unit>();
        Debug.Assert(_unit);

#if DEBUG
        if (_pathVisualization == null)
        {
            var line = new GameObject("Path");
            _pathVisualization = line.AddComponent<LineRenderer>();
            _pathVisualization.widthMultiplier = 0.2f;
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (_path != null)
        {
            if (_pathIndex == _path.Count - 1) // We reached end of path
                Stop();
            else
                FollowPath();
        }
    }

    public void MoveToBlock(BlockData targetBlock)
    {
        PathfindMaster.Instance.RequestPathfind(_unit.GetCurrentBlock(), targetBlock, SetPath);

        lookingForPath = true;
    }

    public void Stop()
    {
        ClearPath();
    }

    public void SetPath(List<BlockData> path)
    {
        lookingForPath = false;

        if (path.Count == 0)
        {
            Debug.LogError("Could not find path");
            return;
        }

        _path = path;
        _path.RemoveAt(_path.Count - 1); // Remove last block, since we don't need to move inside the target object.

        VisualizePath();
    }

    private void FollowPath()
    {
        if (_path == null
         || _path.Count == 0)
            return;

        BlockData targetBlock = _path[_pathIndex + 1];

        Vector3 targetPos = BlockCode.GetSurfaceWorldPos(targetBlock) + GameObjectUtil.GetPivotToMeshMinOffset(_unit.gameObject);
        Vector3 characterToTarget = targetPos - _unit.transform.position;
        Vector3 direction = characterToTarget.normalized;
        Vector3 move = direction * _unit.moveSpeed * Time.fixedDeltaTime;

        _unit.transform.position += move;
        direction.y = 0; // For rotation.
        _unit.transform.rotation = Quaternion.LookRotation(direction);

        float distanceToTarget = characterToTarget.magnitude;
        if (distanceToTarget < 0.1f) // If distance to center of target block is this small, we're good.
            _pathIndex++;
    }

    private void ClearPath()
    {
        _pathIndex = 0;
        if (_path != null)
        {
            _path.Clear();
            _path = null;
        }

        _pathVisualization.positionCount = 0;
    }

    private void VisualizePath()
    {
        _pathVisualization.positionCount = _path.Count;
        for (int i = 0; i < _path.Count; i++)
        {
            _pathVisualization.SetPosition(i, _path[i].worldPosition + Vector3.up);
        }
    }
}
