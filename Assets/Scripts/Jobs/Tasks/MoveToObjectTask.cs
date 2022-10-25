using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using World.Block;

public class MoveToObjectTask : Task
{
    public GameObject targetObject;

    private List<BlockData> _path = null;
    private int _pathIndex = 0;

    public bool lookingForPath { get; private set; } = false;

    public MoveToObjectTask(Job job) : base(job) { }

    private LineRenderer _pathVisualization = null;

    public override void Start()
    {
        base.Start();

        FindPath();

        if(_pathVisualization == null)
        {
            var line = new GameObject("Path");
            _pathVisualization = line.AddComponent<LineRenderer>();
            _pathVisualization.widthMultiplier = 0.2f;
        }
    }

    public override void Tick()
    {
        base.Tick();

        if (_path == null
         && targetObject != null
         && !lookingForPath) // Path find runs on separate thread so don't find new path if already looking.
        {
            FindPath();
        }
        else if (_path != null)
        {
            if (_pathIndex == _path.Count - 1) // We reached end of path
                Finish();
            else
                FollowPath();
        }
    }

    public override void Finish()
    {
        base.Finish();

        ClearPath();
    }

    protected void FindPath()
    {
        Vector3 targetPos = GameObjectUtil.GetObjectBottomPosition(targetObject);
        BlockData targetBlock = GameManager.Instance.World.GetSurfaceBlockUnder(targetPos);

        PathfindMaster.Instance.RequestPathfind(job.unit.GetCurrentBlock(), targetBlock, SetPath);

        lookingForPath = true;
    }

    public void SetPath(List<BlockData> path)
    {
        lookingForPath = false;

        if (path.Count == 0)
        {
            Debug.LogError("Could not find path to " + targetObject.name);
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

        Vector3 targetPos = targetBlock.GetSurfaceWorldPos() + GameObjectUtil.GetPivotToMeshMinOffset(job.unit.gameObject);
        Vector3 characterToTarget = targetPos - job.unit.transform.position;
        Vector3 direction = characterToTarget.normalized;
        Vector3 move = direction * job.unit.moveSpeed * Time.fixedDeltaTime;

        job.unit.transform.position += move;
        direction.y = 0; // For rotation.
        job.unit.transform.rotation = Quaternion.LookRotation(direction);

        float distanceToTarget = characterToTarget.magnitude;
        if (distanceToTarget < 0.1f) // If distance to center of target block is this small, we're good.
            _pathIndex++;
    }

    public void ClearPath()
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
