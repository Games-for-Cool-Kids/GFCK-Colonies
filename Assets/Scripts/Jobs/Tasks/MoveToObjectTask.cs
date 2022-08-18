using UnityEngine;

public class MoveToObjectTask : Task
{
    protected ComponentMove unitMoveComponent;

    public GameObject TargetObject;

    public MoveToObjectTask(Job job) : base(job) { }


    public override void Start()
    {
        base.Start();

        unitMoveComponent = GameObject.FindObjectOfType<ComponentMove>();
        Debug.Assert(unitMoveComponent != null);

        GoToTargetObject();
    }

    public override void Finish()
    {
        base.Finish();

        Stop();
    }

    protected void GoToTargetObject()
    {
        Vector3 targetPos = GameObjectUtil.GetObjectBottomPosition(TargetObject);
        BlockData targetBlock = GameManager.Instance.World.GetSurfaceBlockUnder(targetPos);

        unitMoveComponent.MoveToBlock(targetBlock);
    }

    protected void Stop()
    {
        unitMoveComponent.Stop();
    }
}
