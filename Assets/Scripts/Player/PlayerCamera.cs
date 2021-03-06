using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float hoverHeight = 25.0f;
    public bool hoverWorld = false;

    private Transform _blockFocusPoint;

    private void Start()
    {
        if (hoverWorld)
        {
            _blockFocusPoint = transform.Find("BlockFocusPoint");
            _blockFocusPoint.Translate(Vector3.up * 200, Space.World); // Set high enough to be above the world.
        }
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 direction = Vector3.zero;
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z);
        forward.Normalize();

        direction += transform.right * Input.GetAxis("Horizontal");
        direction += forward * Input.GetAxis("Vertical");

        // Apply move.
        Vector3 move = direction * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

        if (hoverWorld)
            HoverWorld();
    }

    private void HoverWorld()
    {
        Vector3 originalPos = transform.position;
        Vector3 targetHoverPos = transform.position;
        targetHoverPos.y = FindFocusHeight();

        // Set height to hover over focus block.
        Vector3 verticalMove = (targetHoverPos - originalPos) * moveSpeed * Time.deltaTime;
        transform.Translate(verticalMove, Space.World);
    }

    private float FindFocusHeight()
    {
        RaycastHit focusPointHit;
        Physics.Raycast(_blockFocusPoint.position, -Vector3.up, out focusPointHit, 1000, LayerMask.GetMask(GlobalDefines.worldLayerName));

        BlockData focusBlock = GameManager.Instance.World.GetBlockFromRayHit(focusPointHit);
        if (focusBlock == null)
            return hoverHeight;
        else
            return focusBlock.worldPosition.y + hoverHeight;
    }
}
