using UnityEngine;
using System.Collections.Generic;

namespace World
{
    public class GameWorld : MonoBehaviour
    {
        public GameWorldChunkData worldChunks = new();

        public Material material;

        public GameObject TreePrefab;

        public WorldVariable worldVariable;
        public GameObject[,] chunkObjects;

        private WorldGenerator worldGenerator = null; // ToDo: Maybe only create world after generation is finished, so that we can separate generator from world.

        public delegate void WorldGenerationEvent();
        public event WorldGenerationEvent WorldGenerationDone;

        public delegate void BlockEvent(Block block);
        public event BlockEvent blockAdd; // ToDo: update paths that intersect this block. (also diagonal)
        public event BlockEvent blockDig; // ToDo: update paths that intersect this block. (also diagonal)

        private void Start()
        {
            StartCreateWorld();
        }

        void Update()
        {
            if (worldGenerator != null)
                RunWorldGeneration();
        }

        public void StartCreateWorld()
        {
            ResetWorld();

            worldChunks.worldChunkWidth = Mathf.FloorToInt(worldVariable.size / worldChunks.chunkSize);
            worldChunks.blockHeight = worldVariable.height;

            worldGenerator = new(material, worldVariable, TakeGeneratedWorld); // Creates world using multithreading. We need to wait for it to finish to use the world.
        }

        public void RunWorldGeneration()
        {
            if (worldGenerator.worldGenCompleted)
                worldGenerator.NotifyCompleted();
            else
                worldGenerator.Run();
        }

        private void TakeGeneratedWorld(Chunk[,] generatedChunks)
        {
            worldChunks.chunks = generatedChunks;
            chunkObjects = new GameObject[worldChunks.worldChunkWidth, worldChunks.worldChunkWidth];

            foreach (var chunk in generatedChunks)
            {
                var chunkObject = CreateChunkObject(chunk);
                CreateChunkResourceNodes(chunk, chunkObject);
            }

            worldGenerator = null; // Stops generator from running.

            WorldGenerationDone?.Invoke();
        }

        private GameObject CreateChunkObject(Chunk chunk)
        {
            GameObject newChunkObject = new GameObject("Chunk" + chunk.origin.ToString());
            newChunkObject.isStatic = true;
            newChunkObject.layer = LayerMask.NameToLayer(GlobalDefines.worldLayerName);
            newChunkObject.transform.parent = transform;
            newChunkObject.transform.position = chunk.origin;
            chunkObjects[chunk.x, chunk.z] = newChunkObject;

            MeshFilter meshFilter = newChunkObject.AddComponent<MeshFilter>();
            meshFilter.mesh = chunk.TakeMesh();

            MeshRenderer renderer = newChunkObject.AddComponent<MeshRenderer>();
            renderer.material = material;

            newChunkObject.AddComponent<MeshCollider>();

            return newChunkObject;
        }

        private void CreateChunkResourceNodes(Chunk chunk, GameObject chunkObject)
        {
            int startX = chunk.x * worldChunks.chunkSize;
            int startZ = chunk.z * worldChunks.chunkSize;
            for (int x = startX; x < startX + chunk.MaxX; x++)
            {
                for (int z = startZ; z < startZ + chunk.MaxZ; z++)
                {
                    var resource = worldVariable.resourceGrid[x, z];
                    var node = worldVariable.blockGrid[x, z];

                    if (resource.type == ResourceType.RESOURCE_WOOD)
                    {
                        Vector3 blockWorldPos = new Vector3(node.x, node.height, node.y);
                        GameObject.Instantiate(TreePrefab, blockWorldPos, Quaternion.identity, chunkObject.transform);
                    }
                }
            }
        }

        private void ResetWorld()
        {
            worldChunks.chunks = null;

            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public Block GetBlockFromRayHit(RaycastHit hit)
        {
            // Rays intersect with surface. Because the surface is touching, but not inside the box, we need to use the normal to check the position inside the block.
            return GetBlockAt(hit.point - hit.normal * 0.1f);
        }

        /// <summary>Expects a position inside of the block.</summary>
        public Block GetBlockAt(Vector3 worldPos)
        {
            return worldChunks.GetBlockAt(worldPos);
        }

        public Block GetBlockUnderMouse(bool ignoreOtherLayers = false)
        {
            if (UIUtil.IsMouseOverUI()) // Always ignore UI
                return null;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            LayerMask layerMask = ignoreOtherLayers ? LayerMask.GetMask(GlobalDefines.worldLayerName) : ~0;

            if (Physics.Raycast(ray, out hit, 1000, layerMask))
                return GetBlockFromRayHit(hit);

            return null;
        }

        public void DigBlock(Block block)
        {
            if (block.y <= 0) // Cannot destroy bottom-most block.
                return;

            Chunk chunk = worldChunks.GetChunkAt(block.worldPosition);
            chunk.RemoveBlock(block, worldChunks);

            InvokeBlockDigEvent(block);

            UpdateChunkMesh(chunk);
        }

        public void AddBlock(Vector3 worldPos)
        {
            Chunk chunk = worldChunks.GetChunkAt(worldPos);

            Vector3 localPos = chunk.GetLocalPos(worldPos);
            if (localPos.y > worldChunks.blockHeight - 1)
                return;

            Block newBlock = BlockFactory.CreateBlock(localPos, BlockType.GROUND, worldPos);
            chunk.AddBlock(newBlock, worldChunks);

            InvokeBlockAddEvent(newBlock);

            UpdateChunkMesh(chunk);
        }

        private void UpdateChunkMesh(Chunk chunk)
        {
            if (chunk.meshChanged)
            {
                GameObject chunkObject = chunkObjects[chunk.x, chunk.z];
                Mesh chunkMesh = chunk.TakeMesh();
                chunkObject.GetComponent<MeshFilter>().mesh = chunkMesh;
                chunkObject.GetComponent<MeshCollider>().sharedMesh = chunkMesh;
            }
        }

        private void UpdateChangedChunkMeshes(Block block)
        {
            foreach (var chunk in worldChunks.chunks)
                UpdateChunkMesh(chunk);
        }

        public void InvokeBlockAddEvent(Block block)
        {
            blockAdd?.Invoke(block);
        }

        public void InvokeBlockDigEvent(Block block)
        {
            blockDig?.Invoke(block);
        }

        /// <summary>Returns all blocks on the min-y of the bounds.</summary>
        public Block[,] GetBlocksWithinBoundsFloor(Bounds bounds)
        {
            Block[,] result = new Block[Mathf.FloorToInt(bounds.size.x), Mathf.FloorToInt(bounds.size.z)];

            int x_start = Mathf.FloorToInt(bounds.min.x);
            int y = Mathf.FloorToInt(bounds.min.y);
            int z_start = Mathf.FloorToInt(bounds.min.z);

            for (int x = x_start; x < Mathf.FloorToInt(bounds.max.x); x++)
            {
                for (int z = z_start; z < Mathf.FloorToInt(bounds.max.z); z++)
                {
                    result[x - x_start, z - z_start] = GetBlockAt(new Vector3(x, y, z));
                }
            }

            return result;
        }

        public Block GetSurfaceBlockUnder(Vector3 worldPos)
        {
            Chunk chunk = worldChunks.GetChunkAt(worldPos);

            if (chunk == null)
                return null;

            Vector3 localBlockPos = worldPos - chunk.origin;
            int x = Mathf.FloorToInt(localBlockPos.x);
            int startY = Mathf.FloorToInt(worldPos.y); // Start at given y, in case there is overlap.
            int z = Mathf.FloorToInt(localBlockPos.z);

            for (int y = startY; y > 0; y--) // Search from top-down until we hit a surface block.
            {
                Block block = chunk.GetBlock(x, y, z);
                if (block.IsSolidBlock())
                    return block;
            }

            return null;

        }

        public List<Block> GetSurroundingBlocks(Vector3 worldPos, bool diagonal = true, bool includeAir = false)
        {
            List<Block> neighbors = new List<Block>();

            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && z == 0) // Skip self.
                        continue;

                    if (!diagonal // Skip diagonal blocks.
                     && Mathf.Abs(x) == 1
                     && Mathf.Abs(z) == 1)
                        continue;

                    Vector3 searchPos = worldPos + new Vector3(x, 0, z);
                    Block neighbor = GetBlockAt(searchPos);
                    if (neighbor != null)
                    {
                        if (!includeAir
                         && neighbor.type == BlockType.AIR)
                            continue;

                        neighbors.Add(neighbor);
                    }
                }
            }
            return neighbors;
        }
    }
}
