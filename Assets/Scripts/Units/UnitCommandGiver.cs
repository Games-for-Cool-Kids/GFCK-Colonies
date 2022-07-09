using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] LayerMask layermask = new LayerMask();
    [SerializeField] PlayerHand hand = null;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layermask)) { return; }

        Debug.Log("Try to move");
        TryMove(hit.point);
    }

    private void TryMove(Vector3 point)
    {
        foreach (MilitaryUnit unit in hand.SelectedUnits)
        {
            unit.GetUnitMovement().Move(point);
        }
    }
}
