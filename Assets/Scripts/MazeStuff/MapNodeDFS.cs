using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapNodeDFS : IEquatable<MapNodeDFS>
{
    //Coords for tile
    public int x, z;
    //Border Wall booleans
    public int up, down, left, right;
    //If enemy or weapon here
    public bool hasEnemyorWeap;

    public MapNodeDFS(int hori, int vert)
    {
        x = hori;
        z = vert;
        up = down = left = right = 1;
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
