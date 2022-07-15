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
        Destroy(_testCube.GetComponent<BoxCollider>());
    }

    // Update is called once per frame
    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000))
        {
            var block = _world.GetBlockFromRayHit(hit);
            if (block != null)
            {
                _testCube.transform.position = block.worldPosition + Vector3.up / 4;
                _testCube.SetActive(true);
            }
            else
                _testCube.SetActive(false);
        }
    }
}
