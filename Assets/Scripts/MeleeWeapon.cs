using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    //Script for melee weapons
    [Header("----- Components -----")]
    [SerializeField] Rigidbody rb;

    [Header("----- Stats -----")]
    [SerializeField] int damage;
    int attackDelay;
    bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    //Detects collision with another body
    private void OnTriggerEnter(Collider other)
    {
        //Ignores triggers
        if (other.isTrigger)
        {
            return;
        }

        if(!isAttacking)
        {
            StartCoroutine(Attack(other));
        }
    }

    IEnumerator Attack(Collider other)
    {
        isAttacking = true;

        //Check for damageable object
        IDamage damageable = other.GetComponent<IDamage>();

        //Checks to be certain that damageable object is player (for now)
        if (damageable != null && other.CompareTag("Player"))
        {
            damageable.TakeDamage(damage);
        }

        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
    }

    public void SetAttackDelay(int input)
    {
        attackDelay = input;
    }
}
