using UnityEngine;

public class ShowHoveredBlock : MonoBehaviour
{
    private World _world;
    private GameObject _testCube;

    // Start is called before the first frame update
    void Start()
    {
        _world = GetComponent<World>();

        _testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _testCube.name = "HoveredBlock";
        Destroy(_testCube.GetComponent<BoxCollider>());
    }

    // Update is called once per frame
    void Update()
    {
        var block = _world.GetBlockUnderMouse();

        if (block != null)
        {
            _testCube.transform.position = block.worldPosition + Vector3.up / 4;
            _testCube.SetActive(true);
        }
        else
            _testCube.SetActive(false);
    }
}
