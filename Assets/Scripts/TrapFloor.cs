using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapFloor : MonoBehaviour
{
    //add an optional delay to simulate wood breaking
    public float delayBeforeDisappearing = 0f;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Invoke(nameof(Disappear), delayBeforeDisappearing);
        }
    }

    //Enable Trap Floor
    private void Disappear()
    {
        Destroy(gameObject);
    }
}
