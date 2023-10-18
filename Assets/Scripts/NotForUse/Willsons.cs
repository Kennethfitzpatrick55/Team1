//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class Willsons : Maze
//{
//    List<MapLocation> directions = new List<MapLocation>()
//    {
//         new MapLocation(1,0),//up
//         new MapLocation(0,1),//down
//         new MapLocation(-1,0),//left
//         new MapLocation(0,-1),//rigth
//    };
    
//    List<MapLocation> notUsed = new List<MapLocation>();
//    public override void Generate()
//    {
//        // create a starting cell postion at random 
//        int x = Random.Range(2, width - 1);
//        int z = Random.Range(2, depth - 1);
//        int avaibelcell = GetAvailbeilCells();
//        map[x, z] = 2;
//        //for (int i = 0;i < 10;i++) 
//        while(avaibelcell > 1)
//        { 
//            RandomWalk();
//            avaibelcell = GetAvailbeilCells();
//        }
       
//    }

//    int CountSquareMazeNeighbours(int x, int z)
//    {
//        //int count = 0;
//        //if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;// value to check if by edge of map 
//        //if (map[x - 1, z] == 2) count++;//left 
//        //if (map[x + 1, z] == 2) count++;//right
//        //if (map[x, z + 1] == 2) count++;//up 
//        //if (map[x, z - 1] == 2) count++;//down

//        //return count;
//        int count = 0;
//        for (int d = 0; d < directions.Count; d++)
//        {
//            int nextx = x + directions[d].x;
//            int nextz = z + directions[d].z;
//            if (map[nextx, nextz] == 2)
//            {
//                count++;
//            }
//        }
//        return count;
       
//    }

//    int GetAvailbeilCells()
//    {
//        notUsed.Clear();
//        for (int z = 1; z < depth - 1; z++) 
//        {
//              for (int x = 1; x < width - 1; x++)
//              {

//                if(CountSquareMazeNeighbours(x,z)==0) 
//                {
//                   notUsed.Add(new MapLocation(x,z));
//                }
//              }
            
//        }
//        return notUsed.Count;
//    }
        

//    void RandomWalk()
//    {
//        List<MapLocation>inwalk = new List<MapLocation>();
//        int currentx;
//        int currentz;
//        int rstartIndex = Random.Range(0, notUsed.Count);
        
//        currentx = notUsed[rstartIndex].x;
//        currentz = notUsed[rstartIndex].z;
//        inwalk.Add(new MapLocation(currentx, currentz));

//        int loop = 0;
//        bool vaildPath = false;
//        while(currentx > 0 && currentx < width - 1 && currentz > 0 && currentz < depth -1 && loop < 500  && !vaildPath) 
//        {
            
//            map[currentx, currentz] = 0;
//            if (CountSquareMazeNeighbours(currentx, currentz) > 1)
//                break;

//            int randomdirection = Random.Range(0, directions.Count);
//            int nextx = currentx + directions[randomdirection].x;
//            int nextz = currentz + directions[randomdirection].z;
//            //looking for less then two n to or three neighbors 
//            //
//           if(CountSquareNeighbours(nextx, nextz)< 2)
//           {
//                currentx = nextx;
//                currentz = nextz;
//                inwalk.Add(new MapLocation(currentx, currentz));
//           }

//            vaildPath = CountSquareMazeNeighbours(currentx, currentz) == 1;

//            loop++;
//        }

//        if (vaildPath)
//        {
//            map[currentx, currentz] = 0;
//            Debug.Log("PathFound");

//            foreach (MapLocation m in inwalk)
//            {
//                map[m.x, m.z] = 2;
//            }
//            inwalk.Clear();
//        }
//        else
//        {
//            Debug.Log("Path not found");
//            foreach (MapLocation m in inwalk)
//            {
//                map[m.x, m.z] = 1;
//            }
//            inwalk.Clear();
//        }


//    }
//}
