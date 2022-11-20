using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class NodeGrid
    {
        public PathNode[,,] grid;
        public PathNode At(int x, int y, int z)
        {
            return grid[x, y, z];
        }
    }
}
