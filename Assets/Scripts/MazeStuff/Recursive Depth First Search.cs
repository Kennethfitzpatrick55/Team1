using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RecursiveDepthFirstSearch : Maze
{
    //Corner object for filling tiles
    [SerializeField] GameObject corner;

    //Goal reference for placing
    [SerializeField] GameObject goal;

    //Enemy references for spawning
    [SerializeField] GameObject rangedEnemy;
    [SerializeField] GameObject meleeEnemy;
    [SerializeField] GameObject phantomEnemy;
    [SerializeField] GameObject mino;

    //Weapon pickup references for spawning
    [SerializeField] GameObject gem;
    [SerializeField] GameObject lStaff;
    [SerializeField] GameObject fStaff;

    //List of map tiles visited for recursion
    List<MapNodeDFS> visited = new List<MapNodeDFS>();

    //Boolean for goal being spawned
    bool goalSpawned = false;

    //Enemy spawn counts
    int ranged, melee, phantom;


    public void Start()
    {
        Generate();
    }

    public override void Generate()
    {
        //Set starting point
        visited.Add(new MapNodeDFS(width - 1, depth - 1));

        //As neighbors are found, set passage to that neighbor
        Navigate(visited[visited.Count - 1]);

        //Randomize goal position on north or east edge of map
        if (!goalSpawned && Random.Range(0, 99) < 50)
        {
            Instantiate(goal, new Vector3((Random.Range(0, width) * scale) + (scale / 2), 0, ((depth - 1) * scale) + (scale / 2)), Quaternion.identity);
            goalSpawned = true;
        }
        else if (!goalSpawned)
        {
            Instantiate(goal, new Vector3(((width - 1) * scale) + (scale / 2), 0, (Random.Range(0, depth) * scale) + (scale / 2)), Quaternion.identity);
            goalSpawned = true;
        }

        //Randomize enemy spawns
        for(ranged = 0; ranged < GameManager.instance.GetMaxRanged(); ranged++)
        {
            int tileX = Random.Range(1, width);
            int tileZ = Random.Range(1, depth);
            MapNodeDFS check = new MapNodeDFS(tileX, tileZ);
            int index = visited.IndexOf(check);
            if (!visited[index].hasEnemyorWeap)
            {
                Instantiate(rangedEnemy, new Vector3((tileX * scale) + (scale / 2), 0, (tileZ * scale) + (scale / 2)), Quaternion.identity);
                visited[index].hasEnemyorWeap = true;
            }
        }
        for(phantom = 0; phantom < GameManager.instance.GetMaxPhantom(); phantom++)
        {
            int tileX = Random.Range(1, width);
            int tileZ = Random.Range(1, depth);
            MapNodeDFS check = new MapNodeDFS(tileX, tileZ);
            int index = visited.IndexOf(check);
            if (!visited[index].hasEnemyorWeap)
            {
                Instantiate(phantomEnemy, new Vector3((tileX * scale) + (scale / 2), 0, (tileZ * scale) + (scale / 2)), Quaternion.identity);
                visited[index].hasEnemyorWeap = true;
            }
        }
        for(melee = 0; melee < GameManager.instance.GetMaxMelee(); melee++)
        {
            int tileX = Random.Range(1, width);
            int tileZ = Random.Range(1, depth);
            MapNodeDFS check = new MapNodeDFS(tileX, tileZ);
            int index = visited.IndexOf(check);
            if (!visited[index].hasEnemyorWeap)
            {
                Instantiate(meleeEnemy, new Vector3((tileX * scale) + (scale / 2), 0, (tileZ * scale) + (scale / 2)), Quaternion.identity);
                visited[index].hasEnemyorWeap = true;
            }
        }

        //Spawn weapons
        Instantiate(gem, new Vector3(scale * .75f, 1, scale * .75f), Quaternion.identity);
        bool fWeap = false;
        while (!fWeap)
        {
            int tileX = Random.Range(1, width);
            int tileZ = Random.Range(1, depth);
            MapNodeDFS check = new MapNodeDFS(tileX, tileZ);
            int index = visited.IndexOf(check);
            if (!visited[index].hasEnemyorWeap)
            {
                Instantiate(fStaff, new Vector3((tileX * scale) + (scale / 2), 0, (tileZ * scale) + (scale / 2)), Quaternion.identity);
                fWeap = true;
            }
        }
        bool lWeap = false;
        while(!lWeap)
        {
            int tileX = Random.Range(1, width);
            int tileZ = Random.Range(1, depth);
            MapNodeDFS check = new MapNodeDFS(tileX, tileZ);
            int index = visited.IndexOf(check);
            if (!visited[index].hasEnemyorWeap)
            {
                Instantiate(lStaff, new Vector3((tileX * scale) + (scale / 2), 0, (tileZ * scale) + (scale / 2)), Quaternion.identity);
                lWeap = true;
            }
        }
    }

    //Loops through neighbors of passed in node, recursively grabbing each one
    public void Navigate(MapNodeDFS curr)
    {
        List<MapLocation> dir = new List<MapLocation>()
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

        for (int i = 0; i < dir.Count; i++)
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
            if (next.x < 0 || next.x >= width || next.z < 0 || next.z >= depth)
            {
                continue;
            }
            //Check if node has been visited already
            else if (visited.Contains(next))
            {
                continue;
            }
            else
            {
                //Right direction
                if (dir[i].name == "Right")
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

        //Randomize if weapon in this tile

    }

    //Takes in coordinates and sets walls for that tile
    public void SetWalls(MapNodeDFS curr)
    {
        //Set scale for wall lengths and for tile wall positions
        Vector3 wallLength = new Vector3(scale / 10, wall.transform.localScale.y, scale * 0.8f);
        float tileX = curr.x * scale;
        float tileZ = curr.z * scale;

        //Create corner pieces to fill gaps in maze tiles
        corner.transform.localScale = new Vector3(scale / 10, corner.transform.localScale.y, scale / 10);
        Instantiate(corner, new Vector3(tileX + (scale * .05f), corner.transform.localScale.y / 2, tileZ + (scale * .05f)), Quaternion.identity);
        Instantiate(corner, new Vector3(tileX + (scale * .95f), corner.transform.localScale.y / 2, tileZ + (scale * .05f)), Quaternion.identity);
        Instantiate(corner, new Vector3(tileX + (scale * .05f), corner.transform.localScale.y / 2, tileZ + (scale * .95f)), Quaternion.identity);
        Instantiate(corner, new Vector3(tileX + (scale * .95f), corner.transform.localScale.y / 2, tileZ + (scale * .95f)), Quaternion.identity);

        //Lower wall
        if (curr.down)
        {
            GameObject wallS = Instantiate(wall, new Vector3(tileX + (scale / 2), wall.transform.localScale.y / 2, tileZ + (scale * .05f)), Quaternion.Euler(0, 90, 0));
            wallS.transform.localScale = wallLength;
        }
        //Upper wall
        if (curr.up)
        {
            GameObject wallN = Instantiate(wall, new Vector3(tileX + (scale / 2), wall.transform.localScale.y / 2, tileZ + (scale * .95f)), Quaternion.Euler(0, 90, 0));
            wallN.transform.localScale = wallLength;
        }
        //Left wall
        if (curr.left)
        {
            GameObject wallW = Instantiate(wall, new Vector3(tileX + (scale * .05f), wall.transform.localScale.y / 2, tileZ + (scale / 2)), Quaternion.identity);
            wallW.transform.localScale = wallLength;
        }
        //Right wall
        if (curr.right)
        {
            GameObject wallE = Instantiate(wall, new Vector3(tileX + (scale * .95f), wall.transform.localScale.y / 2, tileZ + (scale / 2)), Quaternion.identity);
            wallE.transform.localScale = wallLength;
        }
    }
}
