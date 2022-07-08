using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    private enum PlayerHandState
    {
        HAND_DRAGGING,
        HAND_IDLE,
    }

    public float HoverHeight = 0.25f;
    public float HandDragForce = 10;

    private int _preGrabLayer;
    private float _preGrabRigidBodyDrag; // Before hand grabs object.
    public float GrabbedRigidBodyDrag = 6; // While grabbed.

    private PlayerHandState _interactionState = PlayerHandState.HAND_IDLE;

    private GameObject _selectedObject;

    void Update()
    {
        HandleInput();       
    }

    private void HandleInput()
    {
        switch(_interactionState)
        {
            case PlayerHandState.HAND_IDLE:
                if(Input.GetMouseButtonDown(0))
                {
                    Debug.Assert(_selectedObject == null);

                    Collider clickedCollider = CastMouseRayFromCamera().collider;

                    if (clickedCollider == null)
                        return;

                    if (clickedCollider.CompareTag(GlobalDefines.draggableObjectTag)) // Draggable object.
                    {
                        Debug.Assert(clickedCollider.gameObject.GetComponent<Renderer>());
                        Debug.Assert(clickedCollider.gameObject.GetComponent<Rigidbody>());

                        GrabSelectedObject(clickedCollider.gameObject);

                        _interactionState = PlayerHandState.HAND_DRAGGING;
                    }
                    else if (clickedCollider.CompareTag(GlobalDefines.resourceNodeTag)) // Clickable resource node.
                    {
                        clickedCollider.gameObject.GetComponent<ResourceNode>().SpawnResource();
                    }
                    //else if (clickedCollider.CompareTag(GlobalDefines.droppedResourceTag)) // Clickable dropped resource.
                    //{
                    //    clickedCollider.gameObject.GetComponent<ResourceDropped>().PickUp();
                    //}
                }
                break;

            case PlayerHandState.HAND_DRAGGING:
                Debug.Assert(_selectedObject != null);

                DragSelectedObject();

                if (Input.GetMouseButtonUp(0))
                {
                    var resource = _selectedObject.GetComponent<ResourceDropped>();
                    if(resource)
                    {
                        Collider colliderToDropOn = CastRayDownFromGrabbedObject().collider;

                        if (colliderToDropOn != null)
                        {
                            var stockPile = colliderToDropOn.GetComponent<BuildingStockpile>();
                            if(stockPile)
                            {
                                stockPile.DropOffResource(resource);
                            }
                        }
                            
                    }

                    ReleaseSelectedObject();

                    _interactionState = PlayerHandState.HAND_IDLE;
                }
                break;
            default:
                break;
        }
    }

    private void GrabSelectedObject(GameObject clickedObject)
    {
        _selectedObject = clickedObject;

        _preGrabLayer = _selectedObject.layer;
        _preGrabRigidBodyDrag = GetSelectedObjectRigidBody().drag;

        //Cursor.visible = false;
        _selectedObject.layer = 2; // Ignore raycasts. Default built-in layer
        SetRigidBodyDrag(GrabbedRigidBodyDrag);
        ToggleSelectedObjectGravity(false);
    }

    private void ReleaseSelectedObject()
    { 
        ToggleSelectedObjectGravity(true);
        SetRigidBodyDrag(_preGrabRigidBodyDrag);
        _selectedObject.layer = _preGrabLayer;

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
        Vector3 worldMousePosNear = CameraUtil.GetWorldPosMouseNear();
        Vector3 worldMousePosFar = CameraUtil.GetWorldPosMouseFar();

        RaycastHit rayHit;
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out rayHit);

        return rayHit;
    }

    // Could be cool to do this check when the object actually hits something (after being released), so you can toss things as a player
    private RaycastHit CastRayDownFromGrabbedObject()
    {
        Debug.Assert(_selectedObject);

        // Maybe we should raycast from the hand instead. Check which one feels nicer in game
        Vector3 startPos = _selectedObject.transform.position;
        Vector3 dir = Vector3.down;

        RaycastHit rayHit;
        Physics.Raycast(startPos, dir, out rayHit, 10.0f);

        return rayHit;
    }
}
