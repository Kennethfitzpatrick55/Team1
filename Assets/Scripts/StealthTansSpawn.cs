using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthTansSpawn : MonoBehaviour
{
    [SerializeField] GameObject maze;
    [SerializeField] GameObject stealth;

    // Start is called before the first frame update
    void Start()
    {
        maze.GetComponent<RecursiveDepthFirstSearch>().SetItem(stealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
