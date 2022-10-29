using UnityEngine;
using UnityEngine.InputSystem;
using World;

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
                    unit.gameObject.GetComponent<UnitComponentMove>().MoveToBlock(target_block, null);
                }
            }
        }
    }
}
