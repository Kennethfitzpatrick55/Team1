using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponpickup : MonoBehaviour
{
    
    ////[SerializeField] GameObject button;


    //bool playerInTrigger;
    //void Start()
    //{
    //    Weapon.ammmoCur = Weapon.ammmoMax;
    //}

    //private void Update()
    //{
    //    if (/*Input.GetButtonDown("Interact") && */playerInTrigger)
    //    {

    //        GameManager.instance.playerScript.setWeaponStats(Weapon);
    //        Destroy(gameObject);

    //    }
    //}


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        playerInTrigger = true;
    //        //if (button != null)
    //        //    button.SetActive(true);
    //    }
    //}



    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        //transfwer gun stats to player 
    //        playerInTrigger = false;
    //        //button.SetActive(false);
    //    }
    //}

    [SerializeField]WeaponStats WEAPON;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //transfer gunstates to player
            GameManager.instance.playerScript.setWeaponStats(WEAPON);
            Destroy(gameObject);
        }
    }

}

