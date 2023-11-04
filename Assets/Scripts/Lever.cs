using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    public WaterControl waterRising;
    public MovingPlatform[] platforms;
    public GameObject exitDoor;
    

    private bool isPlayerNear = false;

    private void Update()
    {
        if(isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (waterRising != null)
            {
                Destroy(exitDoor);
                
                waterRising.StartRising();

                
                foreach(var platform in platforms)
                {
                    platform.ActivatePlatform();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
