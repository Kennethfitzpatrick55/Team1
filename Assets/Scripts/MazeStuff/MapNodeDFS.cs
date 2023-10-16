using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodeDFS : IEquatable<MapNodeDFS>
{
    //Coords for tile
    public int x, z;
    //Border Wall booleans
    public bool up, down, left, right;

    public MapNodeDFS(int hori, int vert)
    {
        x = hori;
        z = vert;
        up = down = left = right = true;
    }

    public bool Equals(MapNodeDFS other)
    {
        bool output = false;
        if(x == other.x && z == other.z)
        {
            output = true;
        }
        return output;
    }
}
