using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float moveSpeed = 1f;
    private float defaultMoveFactor = 0.01f; // Used to lower move speed, so we don't have to work with small decimals.

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 direction = Vector3.zero;

        direction.x = Input.GetAxis("Horizontal");
        direction.z = Input.GetAxis("Vertical");

        transform.Translate(direction * moveSpeed * defaultMoveFactor, Space.World);
    }
}
