using System;
using UnityEngine;

public class BuildHand : MonoBehaviour
{
    private GameObject _selectedStructure;

    public Material SelectedBuildingMaterial;
    private Material _selectedStructureOldMaterial;

    public event EventHandler StructurePlaced;
    public event EventHandler BuildCanceled;

    private void Start()
    {

    }

    void Update()
    {
        if(_selectedStructure != null)
            MoveStructureToMousePos();

        if (Input.GetMouseButtonDown(0))
            PlaceStructure();

        if (Input.GetMouseButtonDown(1))
            CancelBuilding();
    }

    private void MoveStructureToMousePos()
    {
        var hovered_block = GameManager.Instance.World.GetBlockUnderMouse();
        if (hovered_block != null)
            _selectedStructure.transform.position = hovered_block.worldPosition;

        if (Input.GetKeyDown(KeyCode.E))
            _selectedStructure.transform.Rotate(Vector3.up, 90);
        if (Input.GetKeyDown(KeyCode.Q))
            _selectedStructure.transform.Rotate(Vector3.up, -90);
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

    private void CancelBuilding()
    {
        GameObject.Destroy(_selectedStructure);
        _selectedStructure = null;

        BuildCanceled.Invoke(this, null);
    }
}
