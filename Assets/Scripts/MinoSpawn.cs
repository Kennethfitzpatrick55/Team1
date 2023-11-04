using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoSpawn : MonoBehaviour
{
    [SerializeField] GameObject pickupText;

    bool playerInTrigger;

    private void Update()
    {
        if (Input.GetButtonDown("Interact") && playerInTrigger)
        {
            SpawnMino();
        }
        transform.Rotate(new Vector3(30, 25, 50) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            pickupText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            pickupText.SetActive(false);
        }
    }

    void SpawnMino()
    {
        GameManager.instance.maze.GetComponent<RecursiveDepthFirstSearch>().EnterBoss();
        Destroy(gameObject);
    }
}
