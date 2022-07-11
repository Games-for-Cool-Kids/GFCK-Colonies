using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHand : MonoBehaviour
{

    private enum PlayerHandState
    {
        HAND_DRAGGING,
        HAND_IDLE
    }

    public float HoverHeight = 0.25f;
    public float HandDragForce = 10;
    public RaycastHit RayHit;

    private int _preGrabLayer;
    private float _preGrabRigidBodyDrag; // Before hand grabs object.
    public float GrabbedRigidBodyDrag = 6; // While grabbed.

    private PlayerHandState _interactionState = PlayerHandState.HAND_IDLE;

    private GameObject _selectedObject;
    private Camera mainCamera;
    private Vector2 startPosition;


    public List<MilitaryUnit> SelectedUnits { get; } = new List<MilitaryUnit>();
    [SerializeField] private RectTransform unitSelectionArea = null;
    [SerializeField] LayerMask layermask = new LayerMask();
    [SerializeField] PlayerInfo playerInfo;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        switch (_interactionState)
        {
            case PlayerHandState.HAND_IDLE:
                if (Input.GetMouseButtonDown(0))
                {
                    HandleLeftClick();
                }
                else if (Mouse.current.leftButton.isPressed)
                {
                    UpdateSelectionArea();
                }
                else if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    ClearSelectedUnits();
                    ClearSelectionArea();
                }
                break;

            case PlayerHandState.HAND_DRAGGING:
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
                var stockPile = colliderToDropOn.GetComponent<BuildingStockpile>();
                if (stockPile)
                {
                    stockPile.DropOffResource(resource);
                }
            }
        }
    }

    private void HandleLeftClick()
    {
        Debug.Assert(_selectedObject == null); // No object should be selected

        Collider clickedCollider = CastMouseRayFromCamera().collider;
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
        StartSelectionArea();
        HandleUnitSelection(clickedCollider);
    }

    void HandleUnitSelection(Collider clickedCollider)
    {
        if (clickedCollider.TryGetComponent<MilitaryUnit>(out MilitaryUnit unit)) // Military unit selection.
        {
            SelectedUnits.Add(unit);
            unit.Select();
            Debug.Log(unit.ToString());
        }
        else if (clickedCollider.TryGetComponent<Terrain>(out Terrain terrain)) // Military unit deselection.
        {
            foreach (MilitaryUnit selectedUnit in SelectedUnits)
            { selectedUnit.Deselect(); }
            SelectedUnits.Clear();
        }
    }

    private void StartSelectionArea()
    {
        foreach (MilitaryUnit SelectedUnit in SelectedUnits)
        {
            SelectedUnit.Deselect();
        }
        SelectedUnits.Clear();

        unitSelectionArea.gameObject.SetActive(true);

        startPosition = Mouse.current.position.ReadValue();

        UpdateSelectionArea();
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;

        unitSelectionArea.sizeDelta = new Vector2(MathF.Abs(areaWidth), MathF.Abs(areaHeight));
        unitSelectionArea.anchoredPosition = startPosition +
            new Vector2(areaWidth / 2, areaHeight / 2);
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

        _interactionState = PlayerHandState.HAND_DRAGGING;
    }

    private void ClearSelectedUnits()
    {
        foreach (MilitaryUnit selectedUnit in SelectedUnits)
        {
            selectedUnit.Deselect();
        }
        SelectedUnits.Clear();
    }

    private void ClearSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(false);

        if (unitSelectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layermask)) { return; }

            if (!hit.collider.TryGetComponent<MilitaryUnit>(out MilitaryUnit unit)) { return; }

            SelectedUnits.Add(unit);

            foreach (MilitaryUnit selectedUnit in SelectedUnits)
            {
                selectedUnit.Select();
            }
            return;
        }

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

        foreach (MilitaryUnit unit in playerInfo.GetMyMilitaryUnits())
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);
            if (screenPosition.x > min.x &&
                screenPosition.x < max.x &&
                screenPosition.y > min.y &&
                screenPosition.y < max.y)
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }

    }

    private void ReleaseSelectedObject()
    { 
        ToggleSelectedObjectGravity(true);
        SetRigidBodyDrag(_preGrabRigidBodyDrag);
        _selectedObject.layer = _preGrabLayer;

        Cursor.visible = true;

        _selectedObject = null;

        _interactionState = PlayerHandState.HAND_IDLE;
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
