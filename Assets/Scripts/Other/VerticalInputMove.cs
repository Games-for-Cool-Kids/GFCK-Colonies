using UnityEngine;

public class VerticalInputMove : MonoBehaviour
{
    public float MoveSpeed = 20.0f;

    void Update()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.Q))
            direction += Vector3.down;
        if (Input.GetKey(KeyCode.E))
            direction += Vector3.up;

        Vector3 move = direction * MoveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }
}
