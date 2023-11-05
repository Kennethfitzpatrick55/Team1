using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RecursiveDepthFirstSearch : Maze
{
    //Corner and floor objects for filling tiles
    [SerializeField] GameObject corner;
    [SerializeField] GameObject floor;
    [SerializeField] GameObject crawlPass;

    //Used to bake navmesh once objects are spawned
    [SerializeField] NavMeshSurface surface;

    //Goal reference for placing
    [SerializeField] GameObject goal;
    [SerializeField] GameObject devCheat;

    //Enemy references for spawning
    [SerializeField] GameObject rangedEnemy;
    [SerializeField] GameObject meleeEnemy;
    [SerializeField] GameObject phantomEnemy;
    [SerializeField] GameObject mino;
    [SerializeField] GameObject treasure;
    [SerializeField] GameObject treasureCheat;

    //Weapon pickup references for spawning
    [SerializeField] GameObject gem;
    [SerializeField] GameObject lStaff;
    [SerializeField] GameObject lCheat;
    [SerializeField] GameObject fStaff;
    [SerializeField] GameObject fCheat;

    //List of map tiles visited for recursion
    List<MapNodeDFS> visited = new List<MapNodeDFS>();

    //Lists for spawned objects info
    List<TileItem> items = new List<TileItem>();
    List<TileItem> enemies = new List<TileItem>();

    //Boolean for goal being spawned
    bool goalSpawned = false;

    //Enemy spawn counts
    int ranged, melee, phantom;

    //Array for passage types
    static int[] pass = { 0, 2, 3 };


    public void Start()
    {
        Generate();
    }

    public override void Generate()
    {
        if (MazeState.instance.maze1 == null)
        {
            //Set starting point
            visited.Add(new MapNodeDFS(width - 1, depth - 1));

            //As neighbors are found, set passage to that neighbor
            Navigate(visited[visited.Count - 1]);

            //Save maze in MazeState
            MazeState.instance.maze1 = visited;

            //Build navmesh so enemies can instantiate without issue
            surface.BuildNavMesh();

            //Populate maze with enemies and save
            SpawnEnemies();
            MazeState.instance.enemies1 = enemies;
        }
        else
        {
            //Load tiles from saved state
            visited = MazeState.instance.maze1;
            for(int i = 0; i < MazeState.instance.maze1.Count; i++)
            {
                SetTile(MazeState.instance.maze1[i]);
            }

            //Build navmesh so enemies can instantiate without issue
            surface.BuildNavMesh();            

            //Load enemies from saved state
            for(int i = 0; i < MazeState.instance.enemies1.Count; i++)
            {
                Instantiate(MazeState.instance.enemies1[i].item, new Vector3((MazeState.instance.enemies1[i].tile.x * scale) + (scale / 2), 2, (MazeState.instance.enemies1[i].tile.z * scale) + (scale / 2)), Quaternion.identity);
            }
        }

        if(MazeState.instance.items1 == null)
        {
            //Populate maze with game objects and save
            SpawnObjects();
            MazeState.instance.items1 = items;            
        }
        else
        {
            //Load objects from saved state
            for (int i = 0; i < MazeState.instance.items1.Count; i++)
            {
                //Check to avoid respawning already aquired gear
                if (MazeState.instance.items1[i].shouldSpawn)
                {
                    Instantiate(MazeState.instance.items1[i].item, new Vector3((MazeState.instance.items1[i].tile.x * scale) + (scale / 2), 2, (MazeState.instance.items1[i].tile.z * scale) + (scale / 2)), Quaternion.identity);
                }
            }
        }
    }

    public void SpawnObjects()
    {
        //Randomize goal position on north or east edge of map
        if (!goalSpawned && Random.Range(0, 99) < 50)
        {
            int tileX = Random.Range(0, width);
            int tileZ = (depth - 1);
            Vector3 pos = new Vector3((tileX * scale) + (scale / 2), 0, (tileZ * scale) + (scale / 2));
            Instantiate(goal, pos, Quaternion.identity);
            Instantiate(devCheat, new Vector3(pos.x, 4, pos.z + 3), Quaternion.identity);
            goalSpawned = true;

            //Save goal position
            items.Add(new TileItem(true, new MapLocation(tileX, tileZ), goal));
        }
        else if (!goalSpawned)
        {
            int tileX = (width - 1);
            int tileZ = Random.Range(0, depth);
            Vector3 pos = new Vector3((tileX * scale) + (scale / 2), 0, (tileZ * scale) + (scale / 2));
            Instantiate(goal, pos, Quaternion.identity);
            Instantiate(devCheat, new Vector3(pos.x + 3, 4, pos.z), Quaternion.identity);
            goalSpawned = true;

            //Save goal position
            items.Add(new TileItem(true, new MapLocation(tileX, tileZ), goal));
        }

        //Spawn weapons
        bool fWeap = false;
        while (!fWeap)
        {
            int tileX = Random.Range(1, width);
            int tileZ = Random.Range(1, depth);
            MapNodeDFS check = new MapNodeDFS(tileX, tileZ);
            int index = visited.IndexOf(check);
            if (!visited[index].hasEnemyorWeap)
            {
                Vector3 pos = new Vector3((tileX * scale) + (scale / 2), 1, (tileZ * scale) + (scale / 2));
                Instantiate(fStaff, pos, Quaternion.identity);
                Instantiate(fCheat, new Vector3(pos.x, 4, pos.z), Quaternion.identity);
                fWeap = true;
                visited[index].hasEnemyorWeap = true;

                //Save fire staff position
                items.Add(new TileItem(true, new MapLocation(tileX, tileZ), fStaff));
            }
        }
        bool lWeap = false;
        while (!lWeap)
        {
            int tileX = Random.Range(1, width);
            int tileZ = Random.Range(1, depth);
            MapNodeDFS check = new MapNodeDFS(tileX, tileZ);
            int index = visited.IndexOf(check);
            if (!visited[index].hasEnemyorWeap)
            {
                Vector3 pos = new Vector3((tileX * scale) + (scale / 2), 1, (tileZ * scale) + (scale / 2));
                Instantiate(lStaff, pos, Quaternion.identity);
                Instantiate(lCheat, new Vector3(pos.x, 4, pos.z), Quaternion.identity);
                lWeap = true;
                visited[index].hasEnemyorWeap = true;

                //Save lightning staff position
                items.Add(new TileItem(true, new MapLocation(tileX, tileZ), lStaff));
            }
        }

        //Mino boss spawn item
        bool treas = false;
        while (!treas)
        {
            int tileX = Random.Range(1, width);
            int tileZ = Random.Range(1, depth);
            MapNodeDFS check = new MapNodeDFS(tileX, tileZ);
            int index = visited.IndexOf(check);
            if (!visited[index].hasEnemyorWeap)
            {
                Vector3 pos = new Vector3((tileX * scale) + (scale / 2), 2, (tileZ * scale) + (scale / 2));
                Instantiate(treasure, pos, Quaternion.identity);
                Instantiate(treasureCheat, new Vector3(pos.x, 6, pos.z), Quaternion.identity);
                treas = true;
                visited[index].hasEnemyorWeap = true;

                //Save treasure position
                items.Add(new TileItem(true, new MapLocation(tileX, tileZ), treasure));
            }
        }
    }

    //Function for spawning enemies
    public void SpawnEnemies()
    {
        //Randomize ranged enemy spawns
        for (ranged = 0; ranged < GameManager.instance.GetMaxRanged(); ranged++)
        {
            int index;
            do
            {
                int tileX = Random.Range(1, width);
                int tileZ = Random.Range(1, depth);
                MapNodeDFS check = new MapNodeDFS(tileX, tileZ);
                index = visited.IndexOf(check);
                if (!visited[index].hasEnemyorWeap)
                {
                    Instantiate(rangedEnemy, new Vector3((tileX * scale) + (scale / 2), 0, (tileZ * scale) + (scale / 2)), Quaternion.identity);
                    visited[index].hasEnemyorWeap = true;
                    enemies.Add(new TileItem(true, new MapLocation(tileX, tileZ), rangedEnemy));
                    break;
                }
            } while (visited[index].hasEnemyorWeap);
        }
        //Randomize phantom spawns
        for (phantom = 0; phantom < GameManager.instance.GetMaxPhantom(); phantom++)
        {
            int index;
            do
            {
                int tileX = Random.Range(1, width);
                int tileZ = Random.Range(1, depth);
                MapNodeDFS check = new MapNodeDFS(tileX, tileZ);
                index = visited.IndexOf(check);
                if (!visited[index].hasEnemyorWeap)
                {
                    Instantiate(phantomEnemy, new Vector3((tileX * scale) + (scale / 2), 0, (tileZ * scale) + (scale / 2)), Quaternion.identity);
                    visited[index].hasEnemyorWeap = true;
                    enemies.Add(new TileItem(true, new MapLocation(tileX, tileZ), phantomEnemy));
                    break;
                }
            }while (visited[index].hasEnemyorWeap);
        }
        //Randomize melee enemy spawns
        for (melee = 0; melee < GameManager.instance.GetMaxMelee(); melee++)
        {
            int index;
            do
            {
                int tileX = Random.Range(1, width);
                int tileZ = Random.Range(1, depth);
                MapNodeDFS check = new MapNodeDFS(tileX, tileZ);
                index = visited.IndexOf(check);
                if (!visited[index].hasEnemyorWeap)
                {
                    Instantiate(meleeEnemy, new Vector3((tileX * scale) + (scale / 2), 0, (tileZ * scale) + (scale / 2)), Quaternion.identity);
                    visited[index].hasEnemyorWeap = true;
                    enemies.Add(new TileItem(true, new MapLocation(tileX, tileZ), meleeEnemy));
                    break;
                }
            } while (visited[index].hasEnemyorWeap);
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
                int index = Random.Range(0, 2);

                //Right direction
                if (dir[i].name == "Right")
                {
                    curr.right = pass[index];
                    next.left = pass[index];
                }
                //Up direction
                else if (dir[i].name == "Up")
                {
                    curr.up = pass[index];
                    next.down = pass[index];
                }
                //Left direction
                else if (dir[i].name == "Left")
                {
                    curr.left = pass[index];
                    next.right = pass[index];
                }
                //Down direction
                else if (dir[i].name == "Down")
                {
                    curr.down = pass[index];
                    next.up = pass[index];
                }
                //Since valid passage, add to visited list
                visited.Add(next);
                //Recursion
                Navigate(next);
            }
        }
        //Create map tile based off of node structure
        SetTile(curr);

        //Randomize if weapon in this tile

    }

    //Takes in coordinates and sets walls for that tile
    public void SetTile(MapNodeDFS curr)
    {
        //Set scale for wall dimensions for tile wall positions
        Vector3 wallLength = new Vector3(scale / 10, wall.transform.localScale.y, scale * 0.8f);

        //Set scale for crawl space dimensions for tile positions
        Vector3 crawlLength = new Vector3(scale / 10, crawlPass.transform.localScale.y, scale * 0.8f);

        //Get height for crawl passages
        float crawlHeight = (crawlPass.transform.localScale.y / 2) + (wall.transform.localScale.y - crawlPass.transform.localScale.y);

        float tileX = curr.x * scale;
        float tileZ = curr.z * scale;

        //Create corner pieces to fill gaps in maze tiles
        corner.transform.localScale = new Vector3(scale / 10, corner.transform.localScale.y, scale / 10);
        Instantiate(corner, new Vector3(tileX + (scale * .05f), corner.transform.localScale.y / 2, tileZ + (scale * .05f)), Quaternion.identity);
        Instantiate(corner, new Vector3(tileX + (scale * .95f), corner.transform.localScale.y / 2, tileZ + (scale * .05f)), Quaternion.identity);
        Instantiate(corner, new Vector3(tileX + (scale * .05f), corner.transform.localScale.y / 2, tileZ + (scale * .95f)), Quaternion.identity);
        Instantiate(corner, new Vector3(tileX + (scale * .95f), corner.transform.localScale.y / 2, tileZ + (scale * .95f)), Quaternion.identity);

        //Put floor in tile
        Instantiate(floor, new Vector3(tileX + (scale / 2), 0, tileZ + (scale / 2)), Quaternion.identity);

        //Lower wall
        if (curr.down == 1)
        {
            GameObject wallS = Instantiate(wall, new Vector3(tileX + (scale / 2), wall.transform.localScale.y / 2, tileZ + (scale * .05f)), Quaternion.Euler(0, 90, 0));
            wallS.transform.localScale = wallLength;
        }
        else if (curr.down == 2)
        {
            GameObject crawlS = Instantiate(crawlPass, new Vector3(tileX + (scale / 2), crawlHeight, tileZ + (scale * .05f)), Quaternion.Euler(0, 90, 0));
            crawlS.transform.localScale = crawlLength;
        }

        //Upper wall
        if (curr.up == 1)
        {
            GameObject wallN = Instantiate(wall, new Vector3(tileX + (scale / 2), wall.transform.localScale.y / 2, tileZ + (scale * .95f)), Quaternion.Euler(0, 90, 0));
            wallN.transform.localScale = wallLength;
        }
        else if (curr.up == 2)
        {
            GameObject crawlN = Instantiate(crawlPass, new Vector3(tileX + (scale / 2), crawlHeight, tileZ + (scale * .95f)), Quaternion.Euler(0, 90, 0));
            crawlN.transform.localScale = crawlLength;
        }

        //Left wall
        if (curr.left == 1)
        {
            GameObject wallW = Instantiate(wall, new Vector3(tileX + (scale * .05f), wall.transform.localScale.y / 2, tileZ + (scale / 2)), Quaternion.identity);
            wallW.transform.localScale = wallLength;
        }
        else if (curr.left == 2)
        {
            GameObject crawlW = Instantiate(crawlPass, new Vector3(tileX + (scale * .05f), crawlHeight, tileZ + (scale / 2)), Quaternion.identity);
            crawlW.transform.localScale = crawlLength;
        }

        //Right wall
        if (curr.right ==1)
        {
            GameObject wallE = Instantiate(wall, new Vector3(tileX + (scale * .95f), wall.transform.localScale.y / 2, tileZ + (scale / 2)), Quaternion.identity);
            wallE.transform.localScale = wallLength;
        }
        else if (curr.right == 2)
        {
            GameObject crawlE = Instantiate(crawlPass, new Vector3(tileX + (scale * .95f), crawlHeight, tileZ + (scale / 2)), Quaternion.identity);
            crawlE.transform.localScale = crawlLength;
        }
    }

    public void EnterBoss()
    {
        Instantiate(mino, new Vector3((Random.Range(0, width) * scale) + (scale / 2), 4, (Random.Range(0, depth) * scale) + (scale / 2)), Quaternion.identity);
    }

    //Function for others to place their work into the maze
    public void SetItem(GameObject item)
    {
        //Loop to spawn item
        int index;
        do
        {
            int tileX = Random.Range(1, width);
            int tileZ = Random.Range(1, depth);
            MapNodeDFS check = new MapNodeDFS(tileX, tileZ);
            index = visited.IndexOf(check);
            //Spawn passed in item only if tile is empty
            if (!visited[index].hasEnemyorWeap)
            {
                Instantiate(item, new Vector3((tileX * scale) + (scale / 2), 0, (tileZ * scale) + (scale / 2)), Quaternion.identity);
                visited[index].hasEnemyorWeap = true;
                //Add item to list
                MazeState.instance.items1.Add(new TileItem(true, new MapLocation(tileX, tileZ), item));
                //items.Add(new TileItem(true, new MapLocation(tileX, tileZ), item));
                break;
            }
        }while (visited[index].hasEnemyorWeap);
    }
}
