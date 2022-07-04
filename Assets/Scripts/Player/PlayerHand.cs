using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public float HoverHeight = 0.25f;
    public float HandDragForce = 10;

    public float PreGrabRigidBodyDrag; // Before hand grabs object.
    public float GrabbedRigidBodyDrag = 6; // While grabbed.

    private GameObject _selectedObject;

    void Update()
    {
        HandleInput();
        
        if (_selectedObject != null)
        {
            DragSelectedObject();
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_selectedObject == null)
            {
                Collider clickedCollider = CastMouseRayFromCamera().collider;

                if (clickedCollider == null)
                    return;

                if (clickedCollider.CompareTag(GlobalDefines.draggableObjectTag) // Draggable object.
                    && IsObjectValidForSelection(clickedCollider))
                {
                    GrabSelectedObject(clickedCollider.gameObject);
                }
                else if (clickedCollider.CompareTag(GlobalDefines.resourceNodeTag)) // Clickable resource node.
                {
                    clickedCollider.gameObject.GetComponent<ResourceNode>().SpawnResource();
                }
            }
            else
            {
                ReleaseSelectedObject();
            }
        }
    }
    
    private bool IsObjectValidForSelection(Collider clickedObject)
    {
        // Rest of code needs these components to work.
        if (clickedObject.gameObject.GetComponent<Renderer>() == null
         || clickedObject.gameObject.GetComponent<Rigidbody>() == null)
            Debug.LogError("Missing required component");

        return clickedObject.gameObject.GetComponent<Renderer>() != null
            && clickedObject.gameObject.GetComponent<Rigidbody>() != null;
    }

    private void GrabSelectedObject(GameObject clickedObject)
    {
        _selectedObject = clickedObject;

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

        _selectedObject = null;
    }

    private void DragSelectedObject()
    {
        Vector3 currentPosition = _selectedObject.transform.position;
        Vector3 targetPosition = CalculateDragTargetPosition();

        Vector3 force = targetPosition - currentPosition; // Don't normalize force, points closer, smaller force, further larger. 
        GetSelectedObjectRigidBody().AddForce(force * HandDragForce);
    }

    private Vector3 CalculateDragTargetPosition()
    {
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(_selectedObject.transform.position).z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);

        return new Vector3(worldPosition.x, GetSelectedObjectYExtent() + HoverHeight, worldPosition.z);
    }

    private float GetSelectedObjectYExtent()
    {
        var renderer = _selectedObject.GetComponent<Renderer>(); // Renderer gives the bounding box in world space.
        return renderer.bounds.size.y;
    }

    private void ToggleSelectedObjectGravity(bool enabled)
    {
        GetSelectedObjectRigidBody().useGravity = enabled;
    }

    private Rigidbody GetSelectedObjectRigidBody()
    {
        return _selectedObject.GetComponent<Rigidbody>();
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
