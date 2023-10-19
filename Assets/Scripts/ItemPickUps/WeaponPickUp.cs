using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    [SerializeField] WeaponStats WEAPON;
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
        GameManager.instance.playerScript.setWeaponStats(WEAPON);
        Destroy(gameObject);
    }
}

