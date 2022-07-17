using UnityEngine;

public class CreateObjectOnClick : MonoBehaviour
{
    public GameObject ObjectToCreate;

    private bool _createAfterClick = false;

    void Update()
    {
        if(_createAfterClick
        && Input.GetMouseButtonDown(0))
        {
            var hovered_block = GameManager.Instance.World.GetBlockUnderMouse(true);
            if (hovered_block != null)
            {
                var new_object = Instantiate(ObjectToCreate);
                new_object.transform.position = hovered_block.GetWorldPositionOnTop() + GameObjectUtil.GetPivotToMeshMinOffset(new_object);
            }

            _createAfterClick = false;
        }
    }

    public void CreateOnNextClick()
    {
        _createAfterClick = true;
    }
}
