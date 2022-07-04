using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float MoveSpeed = 1f;
    private float MoveFactor = 5.0f;

    private void Start()
    {
        Application.targetFrameRate = 25;
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 direction = Vector3.zero;

        direction.x = Input.GetAxis("Horizontal");
        direction.z = Input.GetAxis("Vertical");

        Vector3 move = direction * MoveSpeed * MoveFactor * Time.deltaTime;

        transform.Translate(move, Space.World);
    }
}
