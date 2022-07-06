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

        direction.x = Input.GetAxis("Horizontal");
        direction.z = Input.GetAxis("Vertical");

        Vector3 move = direction * MoveSpeed * Time.deltaTime;

        transform.Translate(move, Space.World);
    }
}
