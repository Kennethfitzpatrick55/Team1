using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    private GameObject arrowPrefab;
    private Transform player;
    float arrowSpeed = 2;

    private bool isPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isPressed)
        {
            Debug.Log("Incoming!");
            SpawnArrow();
        }
    }

    private void SpawnArrow()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        GameObject arrow = Instantiate(arrowPrefab, transform.position + Vector3.up, Quaternion.identity);
        ArrowMovement arrowMovement = arrow.GetComponent<arrowMovement>();
    }

    

}
