using PathFinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World;

namespace Pathfinding
{
    public class PathNode
    {
        public PathNode(Block block)
        {
            this.block = block;
        }
        public Block block;
        public PathNode parent = null;
        public Heuristics heuristics = new();
    }
}
