using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for storing info on the maze(s) for our game
//Set-up info found here: https://gamedev.stackexchange.com/a/133942
public class MazeState : MonoBehaviour
{
    public static MazeState instance;

    //Maze variables to save
    public List<MapNodeDFS> maze1;
    public List<TileItem> items1;
    public List<TileItem> enemies1;
    

    private void Awake()
    {
        //Create instance if not existant already
        if(instance == null)
        {
            instance = this;
        }
        //Destroy this instance if there is already one in existance
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        maze1 = null;
        items1 = null;
        enemies1 = null;

        //Keeps this object as instanced for tracking info between scenes
        DontDestroyOnLoad(gameObject);
    }

    //Public function for removing items from saved state
    public void DisableItem(int index)
    {
        if (index != -1)
        {
            TileItem dis = items1[index];
            dis.shouldSpawn = false;
            items1[index] = dis;
        }
    }

    //Searches for an item to grab the index
    public int FindItem(GameObject obj)
    {
        //Defaults to -1
        int output = -1;
        
        //Find index of item
        for (int i = 0; i < items1.Count; i++)
        {
            if (items1[i].item.CompareTag(obj.tag))
            {
                output = i;
                break;
            }
        }

        return output;
    }
}

//Class to save item/object info and where to spawn them
[System.Serializable]
public class TileItem
{
    public bool shouldSpawn;
    public MapLocation tile;
    public GameObject item;

    public TileItem(bool shouldSpawn, MapLocation tile, GameObject item)
    {
        this.shouldSpawn = shouldSpawn;
        this.tile = tile;
        this.item = item;
    }
}