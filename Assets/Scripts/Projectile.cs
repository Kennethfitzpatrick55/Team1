using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //Script for ranged projectiles
    [Header("----- Components -----")]
    [SerializeField] Rigidbody rb;

    [Header("----- Components -----")]
    [SerializeField] int damage;
    [SerializeField] float speed;
    [SerializeField] int lifeTime;

    void Start()
    {
        //Sets velocity of projectile
        rb.velocity = transform.forward * speed;
        //Projectile "dies" after specified time to reduce total resource load
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Ignores triggers
        if(other.isTrigger)
        {
            return;
        }
        //Check for damageable object
        IDamage damageable = other.GetComponent<IDamage>();
        if(damageable != null )
        {
            damageable.TakeDamage(damage);
        }
        //Projectile "dies" regardless of what it hits
        Destroy(gameObject);
    }

    //Fetches range of a projectile by calculating its lifetime distance
    public float GetRange()
    {
        return speed * lifeTime;
    }
}
