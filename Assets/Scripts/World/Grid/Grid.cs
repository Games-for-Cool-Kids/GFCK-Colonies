using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public static int GRIDUNIT = 1;

    struct CullingPoints
    {
        public Vector3 TopLeft;
        public Vector3 TopRight;
        public Vector3 BotRight;
        public Vector3 BotLeft;
    }

    public int GridWidth = 10;
    public int GridLength = 10;

    private Dictionary<(int, int), GridTile> _tileMap = new Dictionary<(int, int), GridTile>();
    public GameObject TileVisualizationPrefab;

    private Mesh _mesh;

    void Start()
    {
        // Initialize grid
        for(int i = 0; i < GridWidth; i++)
        {
            for(var j = 0; j < GridLength; j++)
            {
                Vector3 offset = new Vector3(i, 0, j); // A unit should be 1.
                _tileMap.Add((i, j), new GridTile(transform.position + offset));
            }
        }

        CreateGridMesh();
    }

    private void CreateGridMesh()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
    }

    //private void Update()
    //{
    //    var cullingPoints = CalculateCullingPoints();

    //    List<GridTile> visibleTiles = GetVisibleTiles();
    //    Debug.Log("Tiles visible: " + visibleTiles.Count);

    //    CreateVisibleMesh();
    //}

    //private CullingPoints CalculateCullingPoints()
    //{
    //    CullingPoints cullPoints = new CullingPoints();
    //    cullPoints.TopLeft = GetRayIntersectionWithTerrain(Camera.main.ViewportPointToRay(new Vector3(0, 0, 0)));
    //    cullPoints.TopRight = GetRayIntersectionWithTerrain(Camera.main.ViewportPointToRay(new Vector3(1, 0, 0)));
    //    cullPoints.BotRight = GetRayIntersectionWithTerrain(Camera.main.ViewportPointToRay(new Vector3(1, 1, 0)));
    //    cullPoints.BotLeft = GetRayIntersectionWithTerrain(Camera.main.ViewportPointToRay(new Vector3(0, 1, 0)));

    //    return cullPoints;
    //}

    //private Vector3 GetRayIntersectionWithTerrain(Ray ray)
    //{
    //    RaycastHit rayHit;
    //    GameManager.Instance.TerrainCollider.Raycast(ray, out rayHit, 1000);

    //    if(rayHit.collider == null)
    //    {
    //        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    //        float enter = 0;
    //        groundPlane.Raycast(ray, out enter);

    //        return ray.GetPoint(enter);
    //    }

    //    return rayHit.point;
    //}

    //private List<GridTile> GetVisibleTiles()
    //{
    //    List<GridTile> visibleTiles = new List<GridTile>();
    //    foreach(var tile in _tileMap.Values)
    //    {
    //        if(tile.IsVisibleByCamera())
    //        {
    //            visibleTiles.Add(tile);
    //        }
    //    }
    //    return visibleTiles;
    //}

    //private void CreateVisibleMesh()
    //{
    //    _mesh.Clear();


    //}
}
