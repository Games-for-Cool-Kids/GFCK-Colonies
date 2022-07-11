using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float MoveSpeed = 10.0f;

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

        Vector3 move = direction * MoveSpeed * Time.deltaTime;

        transform.Translate(move, Space.World);
    }
}
