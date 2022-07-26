using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MoveToObjectTask : Task
{
    public GameObject targetObject;

    private List<BlockData> _path = null;
    private int _pathIndex = 0;

    public bool lookingForPath { get; private set; } = false;

    public MoveToObjectTask(Job job) : base(job) { }

    private GameObject _testCube = null;

    public override void Start()
    {
        base.Start();

        FindPath();

        _testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _testCube.name = "TargetBlock";
        _testCube.layer = Physics.IgnoreRaycastLayer;
        _testCube.transform.localScale = Vector3.one * 1.1f;
    }

    public override void Tick()
    {
        base.Tick();

        if (_path == null
         && !lookingForPath) // Path find runs on separate thread so don't find new path if already looking.
        {
            FindPath();
        }
        else if (_path != null)
        {
            if (_pathIndex == _path.Count - 1) // We reached end of path
            {
                ClearPath(); // Right now we c
                job.StartNextTask();
            }
            else
                FollowPath();
        }
    }

    private void FindPath()
    {
        if (targetObject == null)
            return;

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
    }

    private void FollowPath()
    {
        if (_path == null
         || _path.Count == 0)
            return;

        BlockData targetBlock = _path[_pathIndex + 1];

        Vector3 targetPos = BlockCode.GetSurfaceWorldPos(targetBlock) + GameObjectUtil.GetPivotToMeshMinOffset(targetObject);
        Vector3 characterToTarget = targetPos - job.unit.transform.position;
        Vector3 direction = characterToTarget.normalized;
        Vector3 move = direction * job.unit.speed * Time.fixedDeltaTime;

        _testCube.transform.position = targetBlock.worldPosition;

        job.unit.transform.position += move;

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
    }
}
