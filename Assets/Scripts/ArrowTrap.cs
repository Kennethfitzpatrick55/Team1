using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [Header("----- Components -----")]
    private GameObject arrowPrefab;
    private Transform player;
    float arrowSpeed = 2;

    [Header("----- Bools -----")]
    private bool isPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        // When the player walks over a tagged spot shoot the arrow
        if(other.CompareTag("Player") && !isPressed)
        {
            isPressed = true;
            SpawnArrow();
        }
    }

    private void SpawnArrow()
    {
        //spawns the arrow 
        Vector3 direction = (player.position - transform.position).normalized;
        GameObject arrow = Instantiate(arrowPrefab, transform.position + Vector3.up, Quaternion.identity);
        ArrowMovement arrowMovement = arrow.GetComponent<ArrowMovement>();
    }
}
