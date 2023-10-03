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
        //Weapon destroyed after a certain amount of time
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

        //Weapon destroyed regardless of what it hit
        Destroy(gameObject);
    }

    //Gets range of weapon based on how far it extends
    public float GetRange()
    {
        return gameObject.transform.localScale.z;
    }
}
