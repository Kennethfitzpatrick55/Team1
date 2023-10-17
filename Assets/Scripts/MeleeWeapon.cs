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
    [SerializeField] int attackDelay;
    bool isAttacking;

    // Start is called before the first frame update
    void Start()
    {

    }

    //Rotates downward swing
    public void Animate()
    {
        gameObject.transform.Rotate(0, 15.0f, 0, Space.Self);
    }

    //Resets animation on weapon
    public void ResetAnim()
    {
        gameObject.transform.rotation = Quaternion.identity;
    }

    //Detects collision with another body
    private void OnTriggerEnter(Collider other)
    {
        //Ignores triggers
        if (other.isTrigger)
        {
            return;
        }

        //Check if attack is on cooldown
        if (!isAttacking)
        {

            //Puts delay between triggers of outgoing damage
            StartCoroutine(Attack(other));
        }
    }

    //Sets cooldown for melee damage being applied
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

        //Disable collider to reset collision check
        GetComponent<CapsuleCollider>().enabled = false;

        yield return new WaitForSeconds(3);

        //Re-enable collider to allow for further collision checks
        GetComponent<CapsuleCollider>().enabled = true;

        isAttacking = false;
    }

    public float GetAttackDelay()
    {
        return attackDelay;
    }
}
