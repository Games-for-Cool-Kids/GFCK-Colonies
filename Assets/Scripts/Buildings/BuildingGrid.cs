using UnityEngine;

[System.Serializable]
public class BuildingGrid
{
    public enum Cell
    {
        FREE,
        BUILT,
        ENTRANCE,
        EXIT,
    }

    public int width
    {
        get { return _width; }
        set { _width = value; ResizeGrid(); }
    }
    private int _width = 2;

    public int length
    {
        get { return _length; }
        set { _length = value; ResizeGrid(); }
    }
    private int _length = 2;


    public Cell[,] grid = new Cell[2, 2];


    private void ResizeGrid()
    {
        grid = new Cell[_width, _length];
    }
}
