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
                new_object.transform.position = hovered_block.GetWorldPositionOnTop() + CalculateHeightOffset(new_object); // Offset with half of a block.
            }

            _createAfterClick = false;
        }
    }

    public void CreateOnNextClick()
    {
        _createAfterClick = true;
    }

    private Vector3 CalculateHeightOffset(GameObject newObject) // Difference between object pivot and mesh bottom in world space.
    {
        float offset = 0;

        if (newObject.TryGetComponent<MeshFilter>(out var mesh_filter))
        {
            var mesh = mesh_filter.mesh;
            if (mesh != null)
            {
                offset = -mesh_filter.mesh.bounds.min.y * newObject.transform.localScale.y;
            }
        }

        return Vector3.up * offset;
    }
}
