﻿using UnityEngine;
using System.Collections.Generic;
using PathFinding;

//for more on A* visit
//https://en.wikipedia.org/wiki/A*_search_algorithm
namespace Pathfinding
{
    public class Pathfinder
    {
        class PathNode
        {
            public PathNode(Block block)
            {
                this.block = block;
            }
            public Block block;
            public PathNode parent = null;
            public Heuristics heuristics = new();
        }

        class NodeGrid
        {
            public PathNode[,,] grid;
            public PathNode At(int x, int y, int z)
            {
                return grid[x, y, z];
            }
        }
        private NodeGrid[,] _chunkNodeGrids;
        private Block _startBlock;
        private Block _targetBlock;

        private int worldChunksX;
        private int worldChunksZ;

        private int chunkSize;
        private int maxY;

        private List<Block> _foundPath;

        private bool _allowVertical = true;

        public volatile bool executionFinished = false;
        public PathfindMaster.PathFindingJobComplete completedCallback;

        //Constructor
        public Pathfinder(World world, Block start, Block target, PathfindMaster.PathFindingJobComplete completedCallback)
        {
            this._startBlock = start;
            this._targetBlock = target;
            this.completedCallback = completedCallback;
            this.worldChunksX = world.worldChunksX;
            this.worldChunksZ = world.worldChunksZ;
            this.chunkSize = world.chunkSize;
            this.maxY = world.maxY;

            GenerateChunkNodeGrids(world.chunks);
        }

        private void GenerateChunkNodeGrids(Chunk[,] chunks)
        {
            _chunkNodeGrids = new NodeGrid[worldChunksX, worldChunksZ];

            for (int x = 0; x < worldChunksX; x++)
            {
                for (int z = 0; z < worldChunksZ; z++)
                {
                    var currGrid = chunks[x, z].grid;

                    _chunkNodeGrids[x, z] = new NodeGrid();
                    _chunkNodeGrids[x, z].grid = new PathNode[currGrid.MaxX, currGrid.MaxY, currGrid.MaxZ];
                    foreach (Block filledBlock in currGrid.GetFilledBlocks())
                    {
                        _chunkNodeGrids[x, z].grid[filledBlock.x, filledBlock.y, filledBlock.z] = new PathNode(filledBlock);
                    }
                }
            }
        }

        public void FindPath()
        {
            _foundPath = FindPathActual(_startBlock, _targetBlock);

            executionFinished = true;
        }

        public void NotifyComplete()
        {
            if (completedCallback != null)
            {
                completedCallback(_foundPath);
            }
        }

        private List<Block> FindPathActual(Block start, Block target)
        {
            List<Block> foundPath = new();

            //We need two lists, one for the nodes we need to check and one for the nodes we've already checked
            List<PathNode> openSet = new();
            HashSet<PathNode> closedSet = new();

            //We start adding to the open set
            openSet.Add(new PathNode(start));

            while (openSet.Count > 0)
            {
                PathNode currentNode = openSet[0];

                for (int i = 0; i < openSet.Count; i++)
                {
                    //We check the costs for the current heuristics
                    if (openSet[i].heuristics.fCost < currentNode.heuristics.fCost
                    || (openSet[i].heuristics.fCost == currentNode.heuristics.fCost && openSet[i].heuristics.hCost < currentNode.heuristics.hCost))
                    {
                        if (!currentNode.Equals(openSet[i]))
                        {
                            currentNode = openSet[i];
                        }
                    }
                }

                //we remove the current node from the open set and add to the closed set
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                //if the current node is the target block
                if (currentNode.block == target)
                {
                    //that means we reached our destination, so we are ready to retrace our path
                    foundPath = RetracePath(start, currentNode);
                    break;
                }

                //if we haven't reached our target, we need to look at the neighbours
                var neighbours = GetNeighbours(currentNode, _allowVertical);
                foreach (PathNode neighbour in neighbours)
                {
                    if (!closedSet.Contains(neighbour))
                    {
                        //we create a new movement cost for our neighbours
                        float newMovementCostToNeighbour = currentNode.heuristics.gCost + GetDistance(currentNode.block, neighbour.block);

                        //and if it's lower than the neighbour's cost
                        if (newMovementCostToNeighbour < neighbour.heuristics.gCost || !openSet.Contains(neighbour))
                        {
                            //we calculate the new costs
                            neighbour.heuristics.gCost = newMovementCostToNeighbour;
                            neighbour.heuristics.hCost = GetDistance(neighbour.block, target);
                            //Assign the parent
                            neighbour.parent = currentNode;
                            //And add the neighbour heuristics to the open set
                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                        }
                    }
                }
            }

            //we return the path at the end
            return foundPath;
        }

        private List<Block> RetracePath(Block startBlock, PathNode endNode)
        {
            //Retrace the path, is basically going from the endNode to the startNode
            List<Block> path = new List<Block>();
            PathNode currentNode = endNode;

            while (currentNode.block != startBlock)
            {
                path.Add(currentNode.block);
                //by taking the parentNodes we assigned
                currentNode = currentNode.parent;
            }

            //then we simply reverse the list
            path.Reverse();

            return path;
        }

        private List<PathNode> GetNeighbours(PathNode source, bool allowVertical = false)
        {
            List<PathNode> neighbours = new();

            // Don't look vertically if not allowed.
            int yMin = allowVertical ? -1 : 0;
            int yMax = allowVertical ? 1 : 0;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        if (!(x == 0 && y == 0 && z == 0)) // Skip current node
                        {
                            Vector3 searchPos = source.block.worldPosition
                                + Vector3.right * x
                                + Vector3.up * y
                                + Vector3.forward * z;

                            PathNode walkableNeighbour = GetNeighbourIfWalkable(searchPos, allowVertical, source);
                            if (walkableNeighbour != null)
                                neighbours.Add(walkableNeighbour);
                        }
                    }
                }
            }

            return neighbours;
        }

        private PathNode GetNeighbourIfWalkable(Vector3 worldPos, bool searchTopDown, PathNode sourceNode)
        {
            PathNode neighbour = null;
            PathNode search = null;

            int xDiff = Mathf.FloorToInt(worldPos.x - sourceNode.block.x);
            int zDiff = Mathf.FloorToInt(worldPos.z - sourceNode.block.z);
            bool isDiagonal = Mathf.Abs(xDiff) == 1
                           && Mathf.Abs(zDiff) == 1;

            // Check given neighbour.
            search = GetPathNode(worldPos);
            if (search != null
            && search.block.IsWalkable())
                neighbour = search;

            // If we want to move diagonally, the neighboring orthogonal blocks also need to be walkable.
            // Right now we only allow it if all 4 blocks are on the same height.
            if (isDiagonal)
            {
                search = GetPathNode(worldPos + Vector3.right * xDiff);
                if (search == null
                || !search.block.IsWalkable())
                {
                    neighbour = null;
                }

                search = GetPathNode(worldPos + Vector3.forward * zDiff);
                if (search == null
                || !search.block.IsWalkable())
                {
                    neighbour = null;
                }
            }
            else
            {
                // Check block above given coords.
                if (neighbour == null)
                {
                    search = GetPathNode(worldPos + Vector3.up);
                    if (neighbour != null
                    && neighbour.block.IsWalkable())
                        neighbour = search;
                }

                // Check block below.
                if (neighbour == null)
                {
                    search = GetPathNode(worldPos - Vector3.up);
                    if (neighbour != null
                    && neighbour.block.IsWalkable())
                        neighbour = search;
                }
            }

            return neighbour;
        }

        private PathNode GetPathNode(Vector3 worldPos)
        {
            int c_x = Mathf.FloorToInt(worldPos.x / chunkSize);
            int c_z = Mathf.FloorToInt(worldPos.z / chunkSize);

            if (c_x < 0 || worldPos.y < 0 || c_z < 0
            || c_x >= worldChunksX || worldPos.y >= maxY || c_z >= worldChunksZ)
                return null; // bounds check

            var grid = _chunkNodeGrids[c_x, c_z].grid;
            int x = Mathf.FloorToInt(worldPos.x - c_x * chunkSize);
            int y = Mathf.FloorToInt(worldPos.y);
            int z = Mathf.FloorToInt(worldPos.z - c_z * chunkSize);

            return grid[x, y, z];
        }

        private float GetDistance(Block posA, Block posB)
        {
            float distX = Mathf.Abs(posA.worldPosition.x - posB.worldPosition.x);
            float distZ = Mathf.Abs(posA.worldPosition.z - posB.worldPosition.z);
            float distY = Mathf.Abs(posA.worldPosition.y - posB.worldPosition.y);

            if (distX > distZ)
            {
                return 14 * distZ + 10 * (distX - distZ) + 10 * distY;
            }

            return 14 * distX + 10 * (distZ - distX) + 10 * distY;
        }
    }
}