using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    [SerializeField] WeaponStats weapon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

     private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //transfer gunstates to player
            //gameManger.instance.playerScript.setGunStates(gun);
            Destroy(gameObject);
        }
    }
}
