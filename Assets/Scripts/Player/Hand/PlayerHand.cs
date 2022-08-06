using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHand : MonoBehaviour
{
    public enum PlayerHandState
    {
        GRABBING,
        IDLE,
        SELECTION,
    }

    public float HoverHeight = 0.25f;
    public float HandDragForce = 10;
    public RaycastHit RayHit;

    private int _preGrabLayer;
    private float _preGrabRigidBodyDrag; // Before hand grabs object.
    public float GrabbedRigidBodyDrag = 6; // While grabbed.

    public PlayerHandState currentState = PlayerHandState.IDLE;

    private GameObject _selectedObject;


    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        switch (currentState)
        {
            case PlayerHandState.IDLE:
                if (Input.GetMouseButtonDown(0))
                {
                    HandleObjectClick();
                }
                break;

            case PlayerHandState.GRABBING:
                Debug.Assert(_selectedObject != null);

                DragSelectedObject();

                if (Input.GetMouseButtonUp(0))
                {
                    DropOffResourceIfAble();

                    ReleaseSelectedObject();
                }
                break;

            default:
                break;
        }
    }

    void DropOffResourceIfAble()
    {
        var resource = _selectedObject.GetComponent<Resource>();
        if (resource)
        {
            Collider colliderToDropOn = CastRayDownFromGrabbedObject().collider;

            if (colliderToDropOn != null)
            {
                var stockPile = colliderToDropOn.GetComponent<Stockpile>();
                if (stockPile)
                {
                    stockPile.DropOffResource(resource);
                }
            }
        }
    }

    private void HandleObjectClick()
    {
        Debug.Assert(_selectedObject == null); // No object should be selected

        Collider clickedCollider = CameraUtil.CastMouseRayFromCamera().collider;
        if (clickedCollider == null)
            return;


        if (clickedCollider.CompareTag(GlobalDefines.draggableObjectTag)
         || clickedCollider.CompareTag(GlobalDefines.resourceTag)) // Draggable objects.
        {
            Debug.Assert(clickedCollider.gameObject.GetComponent<Renderer>());
            Debug.Assert(clickedCollider.gameObject.GetComponent<Rigidbody>());

            GrabSelectedObject(clickedCollider.gameObject);

            return;
        }
        
        if (clickedCollider.CompareTag(GlobalDefines.resourceNodeTag)) // Clickable resource node.
        {
            Debug.Assert(clickedCollider.gameObject.GetComponent<ResourceNode>());

            clickedCollider.gameObject.GetComponent<ResourceNode>().SpawnResource();

            return;
        }
    }

    private void GrabSelectedObject(GameObject clickedObject)
    {
        _selectedObject = clickedObject;

        _preGrabLayer = _selectedObject.layer;
        _preGrabRigidBodyDrag = GetSelectedObjectRigidBody().drag;

        //Cursor.visible = false;
        _selectedObject.layer = 2; // Ignore raycasts. Default built-in layer
        GetSelectedObjectRigidBody().drag = GrabbedRigidBodyDrag;
        ToggleSelectedObjectGravity(false);

        currentState = PlayerHandState.GRABBING;
    }

    private void ReleaseSelectedObject()
    { 
        ToggleSelectedObjectGravity(true);
        GetSelectedObjectRigidBody().drag = _preGrabRigidBodyDrag;
        _selectedObject.layer = _preGrabLayer;

        Cursor.visible = true;

        _selectedObject = null;

        currentState = PlayerHandState.IDLE;
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
