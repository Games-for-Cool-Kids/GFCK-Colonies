using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildHand : MonoBehaviour
{
    private GameObject _selectedStructure;

    public Material PreviewMaterial;

    // Properties to restore after placement.
    private Material _oldMaterial;
    private int _oldLayer;


    public event EventHandler StructurePlaced;
    public event EventHandler BuildCanceled;


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
        var hovered_block = GameManager.Instance.World.GetBlockUnderMouse(true);
        if (hovered_block != null)
            _selectedStructure.transform.position = BlockCode.GetSurfaceWorldPos(hovered_block) + GameObjectUtil.GetPivotToMeshMinOffset(_selectedStructure);

        if (Input.GetKeyDown(KeyCode.E)
         || Mouse.current.middleButton.wasPressedThisFrame)
            _selectedStructure.transform.Rotate(Vector3.up, 90);
        if (Input.GetKeyDown(KeyCode.Q))
            _selectedStructure.transform.Rotate(Vector3.up, -90);
    }

    private void PlaceStructure()
    {
        RestoreStructureProperties();

        _selectedStructure.isStatic = true;
        _selectedStructure = null;

        StructurePlaced.Invoke(this, null);
    }

    public void SelectStructure(GameObject structure)
    {
        _selectedStructure = Instantiate(structure);

        // Store properties we change to restore them later.
        _oldMaterial = _selectedStructure.GetComponent<MeshRenderer>().material;
        _oldLayer = _selectedStructure.layer;

        SetTemporaryStructureProperties();
    }

    public void SetTemporaryStructureProperties()
    {
        // Set preview material
        _selectedStructure.GetComponent<MeshRenderer>().material = PreviewMaterial;

        // Set to preview layer, which does not interact with characters.
        _selectedStructure.layer = LayerMask.NameToLayer(GlobalDefines.previewLayerName);
    }
    public void RestoreStructureProperties()
    {
        // Restore material
        _selectedStructure.GetComponent<MeshRenderer>().material = _oldMaterial;

        // Restore layer
        _selectedStructure.layer = _oldLayer;
    }

    private void CancelBuilding()
    {
        GameObject.Destroy(_selectedStructure);
        _selectedStructure = null;

        BuildCanceled.Invoke(this, null);
    }
}
