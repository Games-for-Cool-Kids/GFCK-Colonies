using UnityEngine;

public class GridTile
{
    enum TileType
    {
        EMPTY = 0,
        RESOURCE_NODE = 1,
        STRUCTURE = 2,
    }

    public Vector3 WorldPos { get; private set; }

    public GridTile(Vector3 positionInWorld)
    {
        WorldPos = positionInWorld;
    }

    public bool IsVisibleByCamera()
    {
        Vector3 topLeft = WorldPos + new Vector3(0, 0, Grid.GRIDUNIT);
        Vector3 topRight = WorldPos + new Vector3(Grid.GRIDUNIT, 0, Grid.GRIDUNIT);
        Vector3 botRight = WorldPos + new Vector3(Grid.GRIDUNIT, 0, 0);
        Vector3 botLeft = WorldPos;

        return IsPointVisibleByCamera(topLeft)
            || IsPointVisibleByCamera(topRight)
            || IsPointVisibleByCamera(botRight)
            || IsPointVisibleByCamera(botLeft);
    }

    private bool IsPointVisibleByCamera(Vector3 point) // point should be world position.
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(point);

        return viewportPos.x >= 0 && viewportPos.x <= 1
            && viewportPos.y >= 0 && viewportPos.y <= 1
            && viewportPos.z >= 0; // Z lower than 0 means it's behind the camera
    }
}