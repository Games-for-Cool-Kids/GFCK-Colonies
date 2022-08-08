using System.Collections.Generic;
using UnityEngine;

public class BuildingGridDrawer : MonoBehaviour
{
    public GameObject gridCell;

    private Building _building = null;
    private Vector3 _lastBuildingPos = Vector3.zero;

    private List<GameObject> _cells = new();

    private void Start()
    {
        _building = gameObject.GetComponent<Building>();
        Debug.Assert(_building != null);
    }

    void Update()
    {
        BuildingGrid buildGrid = _building.buildGrid;

        if (_cells.Count != buildGrid.width * buildGrid.length)
            UpdateGridSize();

        if (_lastBuildingPos != _building.transform.position)
        {
            _lastBuildingPos = _building.transform.position;

            Bounds bounds = GameObjectUtil.GetGridBounds(_building.gameObject);

            for (int x = 0; x < buildGrid.width; x++)
            {
                for (int z = 0; z < buildGrid.length; z++)
                {
                    Vector3 meshPivotOffset = Vector3.forward; // Mesh pivot is not in center
                    Vector3 offset = x * Vector3.right + z * Vector3.forward + meshPivotOffset;
                    _cells[x + z * buildGrid.width].transform.position = bounds.min + offset + Vector3.up * 0.1f;
                }
            }
        }
    }

    private void UpdateGridSize()
    {
        Clear();

        foreach (var cell in _building.buildGrid.grid)
        {
            _cells.Add(Instantiate(gridCell, transform, true));
        }
    }

    void Clear()
    {
        foreach(var cell in _cells)
        {
            Destroy(cell);
        }
        _cells.Clear();
        _lastBuildingPos = Vector3.zero;
    }
}
