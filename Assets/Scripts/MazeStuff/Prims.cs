//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Prims : Maze
//{ 
//   public override void Generate()
//   {
        
//        int x = 2;//starting postion of x
//        int z = 2;// starting postion of z

//        map[x, z] = 0;//map hall

//        // list of walls 
//        List<MapLocation> walls = new List<MapLocation>();
//        walls.Add(new MapLocation(x + 1, z));//up
//        walls.Add(new MapLocation(x - 1, z));//down 
//        walls.Add(new MapLocation(x , z + 1 ));//Left 
//        walls.Add(new MapLocation(x , z - 1 ));//Right

//        int countLoops = 0;// safety in case loop dosent finish 

         
//        while(walls.Count > 0 && countLoops < 5000)// loop through all the walls dont exceed this count
//        {
//            int rwall= Random.Range(0, walls.Count); //random wall 
//            x = walls[rwall].x;
//            z = walls[rwall].z;

//            walls.RemoveAt(rwall);// remove wall from last 

//            if(CountSquareNeighbours(x,z) == 1)//take first wall 
//            {
//                map[x,z] = 0;
//                walls.Add(new MapLocation(x + 1, z));//up
//                walls.Add(new MapLocation(x - 1, z));//down 
//                walls.Add(new MapLocation(x, z + 1));//Left 
//                walls.Add(new MapLocation(x, z - 1));//Right
//            }
//            countLoops++;//incremting counter
//        }

//    }

   
//}
