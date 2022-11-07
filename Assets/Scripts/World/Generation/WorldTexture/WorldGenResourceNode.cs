using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenResourceNode
{
    public int x, y;
    public float height;
    public ResourceType type;

    public WorldGenResourceNode(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
