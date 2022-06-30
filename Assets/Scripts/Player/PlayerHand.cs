using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public float HoverHeight = 0.25f;

    private GameObject selectedObject;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedObject == null)
            {
                RaycastHit mouseHit = CastMouseRayFromCamera();
                if (mouseHit.collider != null
                    && mouseHit.collider.CompareTag(GlobalDefines.draggableObjectName))
                {
                    TakeSelectedObject(mouseHit.collider.gameObject);
                }
            }
            else
            {
                ReleaseSelectedObject();
            }
        }

        if (selectedObject != null)
        {
            DragSelectedObject();
        }
    }

    private void TakeSelectedObject(GameObject clickedObject)
    {
        selectedObject = clickedObject;
        Cursor.visible = false;
        ToggleSelectedObjectGravity(false);
    }

    private void ReleaseSelectedObject()
    {
        ToggleSelectedObjectGravity(true);
        Cursor.visible = true;
        selectedObject = null;
    }

    private void DragSelectedObject()
    {
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
        selectedObject.transform.position = new Vector3(worldPosition.x, GetSelectedObjectYExtent() + HoverHeight, worldPosition.z);
    }

    private float GetSelectedObjectYExtent()
    {
        var renderer = selectedObject.GetComponent<Renderer>(); // Renderer gives the bounding box in world space.
        if (renderer == null)
        {
            Debug.LogError("PlayerHand: No Renderer found on selected object.");
            return 0;
        }

        return renderer.bounds.size.y;
    }

    private void ToggleSelectedObjectGravity(bool enabled)
    {
        var rigidBody = selectedObject.GetComponent<Rigidbody>();
        if (rigidBody == null)
        {
            Debug.LogError("PlayerHand: No Rigidbody found on selected object.");
            return; 
        }    

        rigidBody.useGravity = enabled;
    }

    private RaycastHit CastMouseRayFromCamera()
    {
        Vector3 screenMousePosNear = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.nearClipPlane);

        Vector3 screenMousePosFar = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.farClipPlane);

        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);

        RaycastHit rayHit;
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out rayHit);

        return rayHit;
    }
}
