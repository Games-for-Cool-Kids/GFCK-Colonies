using UnityEngine;
using World;

public class CreateObjectOnClick : MonoBehaviour
{
    public GameObject ObjectToCreate;

    private bool _createAfterClick = false;

    void Update()
    {
        if(_createAfterClick
        && Input.GetMouseButtonDown(0))
        {
            var hovered_block = GameManager.Instance.World.GetBlockUnderMouse();
            if (hovered_block != null)
            {
                var new_object = GameManager.Instance.InstantiateGameObject(ObjectToCreate);
                new_object.transform.position = hovered_block.GetSurfaceWorldPos() + new_object.GetPivotYOffset();

                _createAfterClick = false;
            }
        }
    }

    public void CreateOnNextClick()
    {
        _createAfterClick = true;
    }
}
