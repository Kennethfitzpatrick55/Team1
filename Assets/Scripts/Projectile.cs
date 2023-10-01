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
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger)
        {
            return;
        }

        IDamage damageable = other.GetComponent<IDamage>();
        if(damageable != null )
        {
            damageable.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
