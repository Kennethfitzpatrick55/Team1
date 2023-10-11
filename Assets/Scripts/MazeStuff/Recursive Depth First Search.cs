using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RecursiveDepthFirstSearch : Maze
{
    //Wall positions
    Transform up, down, left, right;

    //List of map tiles visited for recursion
    List<MapNodeDFS> visited = new List<MapNodeDFS>();

    static List<MapLocation> dir = new List<MapLocation>()
    {
        //Right direction
        new MapLocation(1,0),
        //Up direction
        new MapLocation(0,1),
        //Left direction
        new MapLocation(-1,0),
        //Down direction
        new MapLocation(0,-1)
    };

    public void Start()
    {
        Generate();
    }

    public override void Generate()
    {
        //Set starting point
        visited.Add(new MapNodeDFS(width, depth));
        //As neighbors are found, set passage to that neighbor
        Navigate(visited[visited.Count - 1]);
        //Create map based off of Navigate
        for(int i = 0; i < visited.Count; i++)
        {
            SetWalls(visited[i]);
        }
    }

    //Loops through neighbors of passed in node, recursively grabbing each one
    public void Navigate(MapNodeDFS curr)
    {
        for(int i  = 0; i < dir.Count; i++)
        {
            //Check neighbor in selected direction
            MapNodeDFS next = new MapNodeDFS(curr.x + dir[i].x, curr.z + dir[i].z);

            //Bounds check x and z coords
            if(next.x < 0 || next.x > width || next.z < 0 || next.z > depth)
            {
                continue;
            }
            //Check if node has been visited already
            else if(visited.Contains(next))
            {
                continue;
            }
            else
            {
                //Right direction
                if(i == 0)
                {
                    curr.right = false;
                    next.left = false;
                }
                //Up direction
                else if (i == 1)
                {
                    curr.up = false;
                    next.down = false;
                }
                //Left direction
                else if (i == 2)
                {
                    curr.left = false;
                    next.right = false;
                }
                //Down direction
                else if (i == 3)
                {
                    curr.down = false;
                    next.up = false;
                }
                //Since valid passage, add to visited list
                visited.Add(next);
                //Recursion
                Navigate(next);
            }
        }
    }

    //Takes in coordinates and sets walls for that tile
    public void SetWalls(MapNodeDFS curr)
    {
        float tileX = curr.x * scale;
        float tileZ = curr.z * scale;
        //Lower wall
        if (curr.down)
        {
            Instantiate(wall, new Vector3(tileX + 5, 3, tileZ), Quaternion.Euler(0, 90, 0));
        }
        //Upper wall
        if (curr.up)
        {
            Instantiate(wall, new Vector3(tileX + 5, 3, tileZ + 9.5f), Quaternion.Euler(0, 90, 0));
        }
        //Left wall
        if (curr.left)
        {
            Instantiate(wall, new Vector3(tileX, 3, tileZ + 5), Quaternion.identity);
        }
        //Right wall
        if (curr.right)
        {
            Instantiate(wall, new Vector3(tileX + 9.5f, 3, tileZ + 5), Quaternion.identity);
        }
    }
}
