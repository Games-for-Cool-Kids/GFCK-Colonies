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
        get 
        { 
            return grid.GetLength(0);
        }
        set 
        { 
            if (width == value || value <= 0)
                return;
            
            ResizeGrid(value, length);
        }
    }
    public int length
    {
        get 
        { 
            return grid.GetLength(1); 
        }
        set 
        { 
            if (length == value || value <= 0) 
                return; 
            
            ResizeGrid(width, value); 
        }
    }

    [SerializeField]
    public Cell[,] grid;


    public BuildingGrid()
    {
        // By default every building has a 2x2 buildGrid.
        grid = new Cell[2, 2];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                grid[x, z] = Cell.BUILT;
            }
        }
    }

    public void ResizeGrid(int width, int length)
    {
        var oldGrid = grid;
        grid = new Cell[width, length];

        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < length; z++)
            {
                if (x < oldGrid.GetLength(0)
                && z < oldGrid.GetLength(1))
                    grid[x, z] = oldGrid[x, z];
                else
                    grid[x, z] = Cell.BUILT;
            }
        }
    }
}
