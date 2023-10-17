using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunPickUp : MonoBehaviour
{
    [SerializeField] WeaponStats gun;
    [SerializeField] GameObject button;


    bool playerInTrigger;
    void Start()
    {
        gun.ammmoCur = gun.ammmoMax;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact") && playerInTrigger)
        {

            GameManager.instance.playerScript.setWeaponStats(gun);
            Destroy(gameObject);

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            if (button != null)
                button.SetActive(true);
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //transfwer gun stats to player 
            playerInTrigger = false;
            button.SetActive(false);
        }
    }
}

