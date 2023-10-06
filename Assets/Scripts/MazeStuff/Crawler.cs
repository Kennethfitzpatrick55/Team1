using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Crawler : Maze
{
    // Start is called before the first frame update
    public override void Generate()
    {
        
        for (int i = 0; i < 3; i++)//Crawl vertical 
        {
            CrawlVertical();
        }

        for (int i = 0; i < 3; i++)//Crawl Horizontal
        {
           CrawlHorizontal();
        }
    }


    void CrawlVertical()
    {  
        bool done = false;//keeps track of if we finished 

        int x = Random.Range(0, width); //starting postioon of x

        int z = 0;//starting postion of 
   
        while (!done)
        {
            map[x, z] = 1; //starting postion 

            if (Random.Range(0, 100) < 50) // moving acrose the map 
            {
                
                x += Random.Range(-1, 2); //set x to 0 cant back track
            } 
            else if (Random.Range(0, 100) < 50)//moving up or down on the z
            {
                z += Random.Range(0, 2);
            }
            // bit minupulation 
            // whats happenf here on x and z  out side the bottom of maze 
            // 0 index out of range 
            done |= (x < 0 || x >= width || z < 0 || z >= depth);
        }
    }


    void CrawlHorizontal()
    {
        bool done = false; //keeps track of if we finished 
        
        int x = 1;//starting postioon of x
                              
        int z = Random.Range(0, depth);//starting postioon of z  

        while (!done)
        {
           
            map[x, z] = 0; //starting postion 
             
            if (Random.Range(0, 100) < 50)// moving acrose the map
            {
               
                x += Random.Range(0, 2); //set x to 0 cant back track 
            } 
            else if (Random.Range(0, 100) < 50)//moving up or down on the z
            {
                z += Random.Range(-1, 2);
            }

            // bit minupulation 
            // whats happenf here on x and z  out side the bottom of maze 
            // 0 index out of range 
            done |= (x < 0 || x >= width || z < 0 || z >= depth);
        }
    }
}
