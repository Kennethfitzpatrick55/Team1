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
        new MapLocation(1,0, "Right"),
        //Up direction
        new MapLocation(0,1, "Up"),
        //Left direction
        new MapLocation(-1,0, "Left"),
        //Down direction
        new MapLocation(0,-1, "Down")
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
    }

    //Loops through neighbors of passed in node, recursively grabbing each one
    public void Navigate(MapNodeDFS curr)
    {
        for(int i  = 0; i < dir.Count; i++)
        {
            //Shuffle currently selected direction
            if (i != dir.Count - 1)
            {
                int nextDir = Random.Range(i, dir.Count);
                MapLocation temp = dir[nextDir];
                dir[nextDir] = dir[i];
                dir[i] = temp;
            }

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
                if(dir[i].name == "Right")
                {
                    curr.right = false;
                    next.left = false;
                }
                //Up direction
                else if (dir[i].name == "Up")
                {
                    curr.up = false;
                    next.down = false;
                }
                //Left direction
                else if (dir[i].name == "Left")
                {
                    curr.left = false;
                    next.right = false;
                }
                //Down direction
                else if (dir[i].name == "Down")
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
        //Create map tile based off of node structure
        SetWalls(curr);
    }

    //Takes in coordinates and sets walls for that tile
    public void SetWalls(MapNodeDFS curr)
    {
        Vector3 wallLength = new Vector3(wall.transform.localScale.x, wall.transform.localScale.y, wall.transform.localScale.z * .8f);
        float tileX = curr.x * scale;
        float tileZ = curr.z * scale;
        //Lower wall
        if (curr.down)
        {
            GameObject wallS = Instantiate(wall, new Vector3(tileX + (scale / 2), 3, tileZ + 0.5f), Quaternion.Euler(0, 90, 0));
            wallS.transform.localScale = wallLength;
        }
        //Upper wall
        if (curr.up)
        {
            GameObject wallN = Instantiate(wall, new Vector3(tileX + (scale / 2), 3, tileZ + 9.5f), Quaternion.Euler(0, 90, 0));
            wallN.transform.localScale = wallLength;
        }
        //Left wall
        if (curr.left)
        {
            GameObject wallW = Instantiate(wall, new Vector3(tileX + 0.5f, 3, tileZ + (scale / 2)), Quaternion.identity);
            wallW.transform.localScale = wallLength;
        }
        //Right wall
        if (curr.right)
        {
            GameObject wallE = Instantiate(wall, new Vector3(tileX + 9.5f, 3, tileZ + (scale / 2)), Quaternion.identity);
            wallE.transform.localScale = wallLength;
        }
    }
}
