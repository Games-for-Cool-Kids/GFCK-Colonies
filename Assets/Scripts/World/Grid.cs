using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    enum GridTile
    {
        EMPTY = 0,
        RESOURCE_NODE = 1,
        STRUCTURE = 2,
    }

    [ReadOnly] public int GridWidth = 1000;
    [ReadOnly] public int GridLength = 1000;

    private Dictionary<(int, int), GridTile> TileMap;

    void Start()
    {
        TileMap = new Dictionary<(int, int), GridTile>();

    }
}
