using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    [SerializeField] WeaponStats weapon;
    [SerializeField] GameObject pickupText;

    bool playerInTrigger;
    void Start()
    {
        playerInTrigger = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact") && playerInTrigger)
        {
            PickUpObject();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            pickupText.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            pickupText.SetActive(true);
        }
    }

    public void PickUpObject()
    {
        //transfer gunstates to player
        GameManager.instance.player.GetComponent<PlayerWeapons>().SetWeaponStats(weapon);

        //Set weapon to no longer respawn when scene is reloaded
        int index = MazeState.instance.FindItem(gameObject);
        MazeState.instance.DisableItem(index);

        Destroy(gameObject);
    }
}

