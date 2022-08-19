using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommander : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            var target_block = GameManager.Instance.World.GetBlockUnderMouse();
            if (target_block != null)
            {
                foreach (var unit in UnitSelector.Instance.SelectedUnits)
                {
                    unit.gameObject.GetComponent<ComponentMove>().MoveToBlock(target_block, null);
                }
            }
        }
    }
}
