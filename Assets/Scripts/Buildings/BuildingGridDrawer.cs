using System.Collections.Generic;
using UnityEngine;

public class BuildingGridDrawer : MonoBehaviour
{
    public GameObject gridCell;

    private Material _gridCellMaterial;
    public Material gridCellErrorMaterial;

    private Building _building = null;
    private Vector3 _lastBuildingPos = Vector3.zero;

    private List<GameObject> _cells = new();

    private static Vector3 _cellDrawOffset = Vector3.up * 0.1f;

    private void Start()
    {
        _building = gameObject.GetComponent<Building>();
        Debug.Assert(_building != null);

        _gridCellMaterial = gridCell.GetComponentInChildren<MeshRenderer>().sharedMaterial;
    }

    void Update()
    {
        BuildingGrid buildGrid = _building.buildGrid;

        int gridSize = buildGrid.width * buildGrid.length;
        if (_cells.Count != gridSize)
        {
            UpdateCellObjects();
        }

        if (_lastBuildingPos != _building.transform.position)
        {
            MoveToBuildingPos();
        }
    }

    private void MoveToBuildingPos()
    {
        _lastBuildingPos = _building.transform.position;

        BuildingGrid buildGrid = _building.buildGrid;
        Bounds bounds = GameObjectUtil.GetGridBounds(_building.gameObject);
        for (int x = 0; x < buildGrid.width; x++)
        {
            for (int z = 0; z < buildGrid.length; z++)
            {
                Vector3 meshPivotOffset = Vector3.forward; // Mesh pivot is not in center
                Vector3 offset = x * Vector3.right + z * Vector3.forward + meshPivotOffset;

                var cellObject = _cells[x + z * buildGrid.width];
                cellObject.transform.position = bounds.min + offset + _cellDrawOffset;
                UpdateCellMaterial(cellObject);
            }
        }
    }

    private void UpdateCellObjects()
    {
        Clear();

        foreach (var cell in _building.buildGrid.grid)
        {
            _cells.Add(Instantiate(gridCell, transform, true));
        }
    }

    private void UpdateCellMaterial(GameObject cellObject)
    {
        Vector3 cellCenter = cellObject.transform.position - Vector3.forward / 2 + Vector3.right / 2;
        var cellBlock = GameManager.Instance.World.GetBlockAt(cellCenter - _cellDrawOffset * 2);

        if (BlockCode.IsBuildable(cellBlock))
            cellObject.GetComponentInChildren<MeshRenderer>().material = _gridCellMaterial;
        else
            cellObject.GetComponentInChildren<MeshRenderer>().material = gridCellErrorMaterial;
    }

    private void Clear()
    {
        foreach (var cell in _cells)
        {
            Destroy(cell);
        }
        _cells.Clear();
        _lastBuildingPos = Vector3.zero;
    }
}
