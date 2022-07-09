using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] LayerMask layermask = new LayerMask();

    private void Start()
    {
        mainCamera = Camera.main;

        layermask.value = LayerMask.NameToLayer("Everything"); // Set to everything for now.
    }

    void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layermask))
            TryMove(hit.point);
    }

    private void TryMove(Vector3 point)
    {
        foreach (MilitaryUnit unit in GameManager.Instance.HandController.PlayerHand.SelectedUnits)
        {
            unit.GetUnitMovement().Move(point);
        }
    }
}
