using System;
using UnityEngine;

public class BuildHand : MonoBehaviour
{
    public GameObject Terrain;

    private GameObject _selectedStructure;

    public Material SelectedBuildingMaterial;
    private Material _selectedStructureOldMaterial;

    public event EventHandler StructurePlaced;

    public float RotationSpeed = 100;

    private void Start()
    {
        if (Terrain == null)
        {
            Terrain = GameObject.Find("Terrain"); // Look for terrain at startup, good enough for now.
            Debug.Assert(Terrain != null);
            Debug.Assert(Terrain.GetComponent<TerrainCollider>() != null);
        }
    }

    void Update()
    {
        if(_selectedStructure != null)
            MoveStructureToMousePos();

        if (Input.GetMouseButtonDown(0))
            PlaceStructure();
    }

    private void MoveStructureToMousePos()
    {
        var terrainMouseRayHit = CastMouseRayFromCamera();
        if(terrainMouseRayHit.collider == null)
            return;

        _selectedStructure.transform.position = terrainMouseRayHit.point;

        if (Input.GetKey(KeyCode.E))
            _selectedStructure.transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.Q))
            _selectedStructure.transform.Rotate(Vector3.up, -RotationSpeed * Time.deltaTime);
    }

    private void PlaceStructure()
    {
        _selectedStructure.GetComponent<MeshRenderer>().material = _selectedStructureOldMaterial; // Restore material
        _selectedStructure = null;

        StructurePlaced.Invoke(this, null);
    }

    public void SelectStructure(GameObject structure)
    {
        _selectedStructure = Instantiate(structure);

        // Set temporary material.
        _selectedStructureOldMaterial = _selectedStructure.GetComponent<MeshRenderer>().material;
        _selectedStructure.GetComponent<MeshRenderer>().material = SelectedBuildingMaterial;
    }

    private RaycastHit CastMouseRayFromCamera()
    {
        RaycastHit rayHit;
        Terrain.GetComponent<TerrainCollider>().Raycast(CameraUtil.GetRayFromCameraToMouse(), out rayHit, 1000);

        return rayHit;
    }

}
