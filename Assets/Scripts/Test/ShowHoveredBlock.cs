using UnityEngine;
using World;

public class ShowHoveredBlock : MonoBehaviour
{
    private GameWorld _world;
    private GameObject _testCube;

    public Material material;

    // Start is called before the first frame update
    void Start()
    {
        _world = GetComponent<GameWorld>();

        _testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _testCube.name = "HoveredBlock";
        _testCube.layer = Physics.IgnoreRaycastLayer;
        _testCube.transform.localScale = Vector3.one * 1.1f;
        _testCube.GetComponent<MeshRenderer>().material = material;

        Destroy(_testCube.GetComponent<BoxCollider>());
    }

    // Update is called once per frame
    void Update()
    {
        var block = _world.GetBlockUnderMouse();

        if (block != null)
        {
            _testCube.transform.position = block.worldPosition;
            _testCube.SetActive(true);
        }
        else
            _testCube.SetActive(false);
    }
}
