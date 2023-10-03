using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    //Script for melee weapons
    [Header("----- Components -----")]
    [SerializeField] Rigidbody rb;

    [Header("----- Stats -----")]
    [SerializeField] int damage;
    [SerializeField] int lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Ignores triggers
        if (other.isTrigger)
        {
            return;
        }
        //Check for damageable object
        IDamage damageable = other.GetComponent<IDamage>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
        //Weapon "dies" regardless of what it hit
        Destroy(gameObject);
    }
}
