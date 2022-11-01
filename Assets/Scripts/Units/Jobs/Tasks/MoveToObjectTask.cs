using UnityEngine;
using Pathfinding;
using World;

namespace Jobs
{
    public class MoveToObjectTask : Task
    {
        protected UnitComponentMove unitMoveComponent;

        public GameObject TargetObject;

        public MoveToObjectTask(Job job) : base(job) { }


        public override void Start()
        {
            base.Start();

            unitMoveComponent = GameObject.FindObjectOfType<UnitComponentMove>();
            Debug.Assert(unitMoveComponent != null);

            GoToTargetObject();
        }

        protected void GoToTargetObject()
        {
            Debug.Assert(TargetObject != null);
            if (TargetObject == null) return;

            Vector3 unitPos = job.GetAssignedUnit().transform.position;
            Block targetBlock = TargetObject.gameObject.GetClosestNeighboringBlock(unitPos);

            unitMoveComponent.MoveToBlock(targetBlock, Finish);
        }

        protected void Stop()
        {
            unitMoveComponent.Stop();
        }
    }
}
