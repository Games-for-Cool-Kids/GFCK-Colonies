using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    void Start()
    {
        Debug.Log(GameManager.Instance.Grid.GridWidth);
    }
}
