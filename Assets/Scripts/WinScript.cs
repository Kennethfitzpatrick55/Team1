using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Call win condition if player hits exit of maze
    private void OnTriggerEnter(Collider other)
    {
        //Ignores triggers
        if(other.isTrigger)
        {
            return;
        }

        if(other.CompareTag("Player"))
        {
            GameManager.instance.youWin();
        }
    }
}
