using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public float HoverHeight = 0.25f;
    public float HandDragForce = 10;

    public float PreGrabRigidBodyDrag; // Before hand grabs object.
    public float GrabbedRigidBodyDrag = 6; // While grabbed.

    private GameObject selectedObject;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedObject == null)
            {
                RaycastHit mouseHit = CastMouseRayFromCamera();
                if (mouseHit.collider != null
                    && IsObjectValidForSelection(mouseHit.collider))
                {
                    GrabSelectedObject(mouseHit.collider.gameObject);
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

    private bool IsObjectValidForSelection(Collider clickedObject)
    {
        if (!clickedObject.CompareTag(GlobalDefines.draggableObjectName))
            return false;

        // Rest of code needs these components to work.
        if (clickedObject.gameObject.GetComponent<Renderer>() == null
         || clickedObject.gameObject.GetComponent<Rigidbody>() == null)
            Debug.LogError("Missing required component");

        return clickedObject.gameObject.GetComponent<Renderer>() != null
            && clickedObject.gameObject.GetComponent<Rigidbody>() != null;
    }

    private void GrabSelectedObject(GameObject clickedObject)
    {
        selectedObject = clickedObject;

        PreGrabRigidBodyDrag = GetSelectedObjectRigidBody().drag;

        //Cursor.visible = false;
        SetRigidBodyDrag(GrabbedRigidBodyDrag);
        ToggleSelectedObjectGravity(false);
    }

    private void ReleaseSelectedObject()
    {
        ToggleSelectedObjectGravity(true);
        SetRigidBodyDrag(PreGrabRigidBodyDrag);
        Cursor.visible = true;

        selectedObject = null;
    }

    private void DragSelectedObject()
    {
        Vector3 currentPosition = selectedObject.transform.position;
        Vector3 targetPosition = CalculateDragTargetPosition();

        Vector3 force = targetPosition - currentPosition; // Don't normalize force, points closer, smaller force, further larger. 
        GetSelectedObjectRigidBody().AddForce(force * HandDragForce);
    }

    private Vector3 CalculateDragTargetPosition()
    {
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);

        return new Vector3(worldPosition.x, GetSelectedObjectYExtent() + HoverHeight, worldPosition.z);
    }

    private float GetSelectedObjectYExtent()
    {
        var renderer = selectedObject.GetComponent<Renderer>(); // Renderer gives the bounding box in world space.
        return renderer.bounds.size.y;
    }

    private void ToggleSelectedObjectGravity(bool enabled)
    {
        GetSelectedObjectRigidBody().useGravity = enabled;
    }

    private Rigidbody GetSelectedObjectRigidBody()
    {
        return selectedObject.GetComponent<Rigidbody>();
    }

    private void SetRigidBodyDrag(float drag)
    {
        GetSelectedObjectRigidBody().drag = drag;
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
