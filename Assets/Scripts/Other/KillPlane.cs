using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Destroys all GameObjects that correspond to the given tags.
// Scaling to world is purely visual. Actual dimensions of plane don't matter, only the y-coordinate. 
public class KillPlane : MonoBehaviour
{
    [TagSelector]
    public string[] tagFilter = new string[] {};

    public bool scaleToWorld = false; // Purely visual.

    void FixedUpdate()
    {
        var tagObjects = new List<GameObject>();
        for (int i = 0; i < tagFilter.Length; i++)
            tagObjects.AddRange(GameObject.FindGameObjectsWithTag(tagFilter[i]));

        foreach (var gameObject in tagObjects)
        {
            if(gameObject.transform.position.y < transform.position.y)
            {
                Debug.Log("Kill plane destroying: " + gameObject.name);
                Destroy(gameObject);
            }
        }
    }
    
    void Start()
    {
        if (scaleToWorld)
            GameManager.Instance.World.WorldGenerationDone += ScaleToWorld;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

    private void ScaleToWorld()
    {
        var worldBounds = GameManager.Instance.World.gameObject.CalculateRecursiveBounds();

        Vector3 scale = worldBounds.size;
        scale.y = .1f; // Planes are very flat!
        transform.localScale = scale;

        Vector3 pos = worldBounds.center;
        pos.y = transform.position.y; // Don't move in y-axis.
        transform.position = pos;
    }
}
