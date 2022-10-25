using UnityEngine;
using World.Block;

namespace World
{
    public class GameWorld : MonoBehaviour
    {
        public class ChunkDimensions
        {
            public int chunkSize;
            public int worldChunkWidth; // Number of chunks in x/z direction.
        }

        public int chunkSize = 32;
        public int worldChunkWidth { get; private set; } // Nr of chunkGrid in worldChunkWidth and length, as world is a square.

        public Material material;

        public WorldVariable worldVariable;
        public ChunkData[,] chunks;
        public GameObject[,] chunkObjects;

        private WorldGenerator worldGenerator = null;

        public delegate void BlockEvent(BlockData block);
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

            worldChunkWidth = Mathf.FloorToInt(worldVariable.size / chunkSize);

            worldGenerator = new(chunkSize, worldChunkWidth, material, worldVariable, TakeGeneratedWorld); // Creates world using multithreading. We need to wait for it to finish to use the world.
        }

        public void RunWorldGeneration()
        {
            if (worldGenerator.worldGenCompleted)
                worldGenerator.NotifyCompleted();
            else
                worldGenerator.Run();
        }

        private void TakeGeneratedWorld(ChunkData[,] generatedChunks)
        {
            chunks = generatedChunks;
            chunkObjects = new GameObject[worldChunkWidth, worldChunkWidth];

            foreach (var chunk in generatedChunks)
            {
                CreateChunkObject(chunk);
            }

            worldGenerator = null; // Stops generator from running.
        }

        private void CreateChunkObject(ChunkData chunk)
        {
            GameObject newChunkObject = new GameObject("ChunkData" + chunk.origin.ToString());
            newChunkObject.isStatic = true;
            newChunkObject.layer = LayerMask.NameToLayer(GlobalDefines.worldLayerName);
            newChunkObject.transform.parent = transform;
            newChunkObject.transform.position = chunk.origin;
            chunkObjects[chunk.x, chunk.z] = newChunkObject;

            MeshFilter meshFilter = newChunkObject.AddComponent<MeshFilter>();
            meshFilter.mesh = ChunkCode.TakeMesh(chunk);

            MeshRenderer renderer = newChunkObject.AddComponent<MeshRenderer>();
            renderer.material = material;

            newChunkObject.AddComponent<MeshCollider>();
        }

        private void ResetWorld()
        {
            chunks = null;

            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public BlockData GetBlockFromRayHit(RaycastHit hit)
        {
            // Rays intersect with surface. Because the surface is touching, but not inside the box, we need to use the normal to check the position inside the block.
            return GetBlockAt(hit.point - hit.normal * 0.1f);
        }

        /// <summary>Expects a position inside of the block.</summary>
        public BlockData GetBlockAt(Vector3 worldPos)
        {
            var chunk = ChunkCode.GetChunkAt(chunks, GetWorldChunkDimensions(), worldPos);
            if (chunk == null)
            {
                return null;
            }

            return ChunkCode.GetBlockAt(chunk, worldPos);
        }

        public ChunkData GetChunk(int x, int z)
        {
            if (x < 0 || x >= worldChunkWidth
            || z < 0 || z >= worldChunkWidth)
                return null;

            return chunks[x, z];
        }

        public BlockData GetBlockUnderMouse(bool ignoreOtherLayers = false)
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

        public BlockData GetSurfaceBlockUnder(Vector3 worldPos)
        {
            return ChunkCode.GetSurfaceBlockUnder(chunks, GetWorldChunkDimensions(), worldPos);
        }

        public void DigBlock(BlockData block)
        {
            ChunkCode.DigBlock(chunks, GetWorldChunkDimensions(), block);

            UpdateChangedChunkMeshes();
        }

        public void AddBlock(Vector3 worldPos)
        {
            var chunkDimensions = GetWorldChunkDimensions();
            ChunkCode.AddBlock(chunks, chunkDimensions, worldPos);

            ChunkData chunk = ChunkCode.GetChunkAt(chunks, chunkDimensions, worldPos);
            UpdateChunkMesh(chunk);
        }

        private void UpdateChunkMesh(ChunkData chunk)
        {
            if (chunk.meshChanged)
            {
                GameObject chunkObject = chunkObjects[chunk.x, chunk.z];
                Mesh chunkMesh = ChunkCode.TakeMesh(chunk);
                chunkObject.GetComponent<MeshFilter>().mesh = chunkMesh;
                chunkObject.GetComponent<MeshCollider>().sharedMesh = chunkMesh;
            }
        }

        private void UpdateChangedChunkMeshes()
        {
            foreach (var chunk in chunks)
                UpdateChunkMesh(chunk);
        }

        private GameWorld.ChunkDimensions GetWorldChunkDimensions()
        {
            return new GameWorld.ChunkDimensions { chunkSize = this.chunkSize, worldChunkWidth = this.worldChunkWidth };
        }

        public void InvokeBlockAddEvent(BlockData block)
        {
            blockAdd?.Invoke(block);
        }

        public void InvokeBlockDigEvent(BlockData block)
        {
            blockDig?.Invoke(block);
        }
    }
}
