using UnityEngine;

public class HarvestResourceTask : Task
{
    private ResourceType _type;
    private ResourceNode _node;

    private float _timeSinceLastHarvest = 0;

    public HarvestResourceTask(Job job, ResourceType type) : base(job)
    {
        this._type = type;
    }

    public override void Start()
    {
        base.Start();

        _node = FindResourceNode();
        if (_node == null)
        {
            Debug.LogError("No node found next to unit! Cannot complete task.");
            Finish();
        }

        _timeSinceLastHarvest = 0;
    }

    public override void Tick()
    {
        base.Tick();

        float timeToHarvest = 1.0f / job.unit.harvestSpeed;
        if(_timeSinceLastHarvest >= timeToHarvest)
        {
            if (_node.Harvest(job.unit.harvestDamage))
            {
                _node.SpawnResource();
                Finish();
            }
        }
        else
        {
            _timeSinceLastHarvest += Time.deltaTime;
        }
    }

    // This function assumes the character is already next to the node, otherwise we will finish the task.
    private ResourceNode FindResourceNode()
    {
        string nodeName;
        switch (_type)
        {
            case ResourceType.RESOURCE_WOOD:
                nodeName = GlobalDefines.treeResourceNodeName;
                break;
            case ResourceType.RESOURCE_STONE:
                nodeName = GlobalDefines.stoneResourceNodeName;
                break;
            case ResourceType.RESOURCE_INVALID:
            default:
                Debug.LogError("No implementation for resource type: " + _type);
                return null;
        }

        foreach (var node in GameObject.FindGameObjectsWithTag(GlobalDefines.resourceNodeTag))
        {
            if (node.name.Contains(nodeName))
            {
                var resourceNode = node.GetComponent<ResourceNode>();

                BlockData nodeBlock = resourceNode.GetBlock();
                BlockData unitBlock = job.unit.GetCurrentBlock();

                float distance = (nodeBlock.worldPosition - unitBlock.worldPosition).magnitude;
                if (distance < 1.45f) // Within one block distance. (also diagonal, see Pythagoras)
                    return resourceNode;
            }
        }

        return null;
    }
}
