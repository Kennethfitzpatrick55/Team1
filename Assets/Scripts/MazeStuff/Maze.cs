using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder.Shapes;


// helper class 
public class MapLocation
{
    //storage 
    public int x;
    public int z;
    public string name;
    //constructer 
    public MapLocation(int _x, int _z, string _name = null)
    {
        x = _x;
        z = _z;
        name = _name;
    }
}

public class Maze : MonoBehaviour
{
    /*
    ...................................................................
    .                                                                 .
    .                     ----(Note)----                              .
    .      ----reading this please dont move my notes)----            .
    .  chrome-extension://efaidnbmnnnibpcajpcglclefindmkaj            .
    . /https://www.cs.cmu.edu/~112/notes/student-tp-guides/Mazes.pdf  . 
    .                                                                 .
    .                                                                 .   
    .       (make cubs alighned                                       .
    .   ______________________>                                       .            
    .  |       ||||||  |||||         for() var++                      .
    .  |       ||||||  |||||           for(var,var++                  .
    .  |       ||||||  |||||       place in space(x,0,z)              .
    .  |       ||||||  |||||  makepbject(object,postion,not rot)      .                  
    .   -------------  --------> print a row of blocks                .
    .                                                                 .
    .                                                                 .
    .  (  were faceing here )                                         .
    .            |                      Check whats next to it        .       
    .     (y)    |                          do somethigng             .
    .      | ____________                    (x,z+1)                  .
    .      | |__|__|__|||| (cordinetes)         up                    .
    .      | |__|__|__||||   (z)                __                    .
    .      | |__|__|__||||    |  (x-,z)Left<---|__|---> Right(x+1,z)  .   
    .      | |__|__|__||||    |__.__.__.__.    Down                   .
    .      -------------      |__|__|__|__|   (x,z-1)                 .
    .                         |__|__|__|__|                           .
    .                         |__|__|__|__|                           .
    .                         |__|__|__|__|                           .
    .                         |__|__|__|__|_____(x)                   .
    .                                                                 .
    .                                                                 .
    .               How to check Vertical                             .
    .                    (x,z+1) (z-1)                                .  
    .                          Up                                     . 
    .                          _|_                                    .  
    .  (x-1,z )(x+1,z)left<---|_0_|---->Right (x+1,z)(x-1,z)          .  
    .                          |  |_0_| ___                           .
    .                          |       |_0_| ___                      . 
    .                    (x,z-1)(z,z+1)     |_0_|                     .
    .                                                                 .
    .                                                                 . 
    .                                                                 . 
    .                                                                 .
    .                                                                 .
    .       ( I want to modfiy this  control the postion ?)           .                 
    .     I need to store the postion of objects;                     .
    .     ._.._.._.._._.._.                                           .
    .     |_1_|_1_|_1_|_1_|                                           . 
    .     |_1_|_1_|_1_|_1_|     (Initilize make suyrface)             .
    .     |_1_|_1_|_1_|_1_|                                           .
    .     |_1_|_1_|_1_|_1_|      1= walls                             .
    .     |_1_|_1_|_1_|_1_|                                           . 
    .                                                                 . 
    .     ._.._.._.._._.._.                                           .
    .     |_0_|_1_|_1_|_1_|    (Generate over ride  to create holes)  . 
    .     |_1_|_0_|_1_|_1_|                                           .
    .     |_0_|_1_|_0_|_1_|     0 = passages                          .
    .     |_1_|_1_|_1_|_1_|                                           .
    .     |_1_|_0_|_1_|_1_|                                           .
    .                                                                 .
    .       side note- this is very familer to the game of life       .
    .       I wonder if mine craft is handeled in a simliar way       .
    .       or if most  terrain generation is like this               .
    .                                                                 .
    .                  (cordinetes)                                   .
    .                                                                 .
    .                      (z)                                        .
    .                       |                                         .
    .                       |                                         .
    .                       |__.__.__.__.                             .
    .                       |__|__|__|__|                             .
    .                       |__|__|__|__|                             .
    .                       |__|__|__|__|                             .
    .                       |__|__|__|__|                             .
    .                       |__|__|__|__|_____(x)                     .
    .          Postion we are always refercing is (x,z)               .
    .               - access to all postions                          .
    .                                                                 .
    ...................................................................
    */

    public int width = 30;// x width
    public int depth = 30;// z length 
    public byte[,] map;  // postion of wall or passageway 
    public int scale = 6;

    //Wall reference for placing
    public GameObject wall;

    // Start is called before the first frame update
    void Start()
    {
        InitialisMap();
        Generate();
        DrawMap();

    }

    void InitialisMap()
    {

        map = new byte[width, depth];
        //setting lenght  on the z
        for (int z = 0; z < depth; z++)
        {
            // setting width  on the x
            for (int x = 0; x < width; x++)
            {
                //This is all 1

                //0,1//
                map[x, z] = 1;  //1= wal  0 = passage

            }

        }

    }

    public virtual void Generate()
    {
        for (int z = 0; z < depth; z++)
        {
            //setting lenght  on the z
            for (int x = 0; x < width; x++)
            {
                // value less then 50
                if (Random.Range(0, 100) < 50)
                {
                    //over ride with 0
                    //0,1//
                    map[x, z] = 0;  //1= wal  0 = passage
                }
            }
        }
    }

    void DrawMap()
    {
        for (int z = 0; z < depth; z++)
        {
            //setting lenght  on the z
            for (int x = 0; x < width; x++)
            {
                if (map[x, z] == 1)
                {
                    //x,z postion
                    //multiply the potion by scale 
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    //Instantiate(cube, pos, Quaternion.identity);
                    //make object
                    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    // move wall to postion 
                    wall.transform.localScale = new Vector3(scale, scale, scale);
                    wall.transform.position = pos;
                }

            }
        }
    }

  
}
