using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelector : MonoBehaviourSingleton<UnitSelector>
{
    public List<MilitaryUnit> SelectedUnits { get; } = new List<MilitaryUnit>();
    public RectTransform _unitSelectionArea = null;

    private Vector2 mouseDragStartPosition;

    private void Start()
    {
        _unitSelectionArea.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleLeftClick();
        }
        if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectedUnits();
            SelectMilitaryUnitsInSelectionArea();
            _unitSelectionArea.gameObject.SetActive(false);
        }
    }
    void OnDisable()
    {
        if(_unitSelectionArea != null) // Can be called at game quit, when component is destroyed.
            _unitSelectionArea.gameObject.SetActive(false);
    }

    private void HandleLeftClick()
    {
        LayerMask mask = LayerMask.NameToLayer(GlobalDefines.characterLayerName);
        mask += LayerMask.NameToLayer(GlobalDefines.worldLayerName);

        Collider clickedCollider = CameraUtil.CastMouseRayFromCamera(/*~mask*/).collider;
        if (clickedCollider != null)
        {
            if (clickedCollider.TryGetComponent(out MilitaryUnit unit))
            {
                SelectedUnits.Add(unit);
                unit.Select();

                return;
            }
        }

        ClearSelectedUnits();

        StartSelectionArea();
    }

    private void StartSelectionArea()
    {
        _unitSelectionArea.gameObject.SetActive(true);

        mouseDragStartPosition = Mouse.current.position.ReadValue();

        UpdateSelectionArea();
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float areaWidth = mousePosition.x - mouseDragStartPosition.x;
        float areaHeight = mousePosition.y - mouseDragStartPosition.y;

        _unitSelectionArea.sizeDelta = new Vector2(MathF.Abs(areaWidth), MathF.Abs(areaHeight));
        _unitSelectionArea.anchoredPosition = mouseDragStartPosition +
            new Vector2(areaWidth / 2, areaHeight / 2);
    }

    private void ClearSelectedUnits()
    {
        foreach (MilitaryUnit selectedUnit in SelectedUnits)
            selectedUnit.Deselect();

        SelectedUnits.Clear();
    }

    private void SelectMilitaryUnitsInSelectionArea()
    {
        if (_unitSelectionArea.sizeDelta.magnitude <= 0.1f)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, PhysicsUtil.GetInverseLayerMaskFromName(GlobalDefines.characterLayerName)))
                return;

            if (!hit.collider.TryGetComponent<MilitaryUnit>(out MilitaryUnit unit))
                return;

            SelectedUnits.Add(unit);

            foreach (var selectedUnit in SelectedUnits)
                selectedUnit.Select();

            return;
        }

        Vector2 min = _unitSelectionArea.anchoredPosition - (_unitSelectionArea.sizeDelta / 2);
        Vector2 max = _unitSelectionArea.anchoredPosition + (_unitSelectionArea.sizeDelta / 2);

        foreach (var unit in PlayerInfo.Instance.playerFaction.MilitaryUnits)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(unit.transform.position);
            if (screenPosition.x > min.x &&
                screenPosition.x < max.x &&
                screenPosition.y > min.y &&
                screenPosition.y < max.y)
            {
                MilitaryUnit milUnit = unit.GetComponent<MilitaryUnit>();
                SelectedUnits.Add(milUnit);
                milUnit.Select();
            }
        }
    }
}
