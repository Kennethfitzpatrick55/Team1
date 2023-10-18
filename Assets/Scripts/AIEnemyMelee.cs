using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AIEnemyMelee : MonoBehaviour, IDamage
{
    //Controller script for melee enemy AI
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Animator anim;
    [SerializeField] Collider hitBox;

    [Header("----- Stats -----")]
    [Range(1, 25)] [SerializeField] int HP;
    [SerializeField] int turnSpeed;
    [Range(1, 180)][SerializeField] int viewAngle;

    [Header("----- Attack Stats -----")]
    [SerializeField] GameObject weapon;
    [SerializeField] int attackDelay;
    [SerializeField] float range;

    bool playerInRange;
    float angleToPlayer;
    Vector3 playerDir;
    Color colorOrig;

    // Start is called before the first frame update
    void Start()
    {
        //Fetches original color to not mess up color/model after damage flash
        colorOrig = model.material.color;

        //Update enemy count with each spawn
        GameManager.instance.UpdateEnemyCount(1);

        //Sends attack delay to weapon so damage can be more consistent
        weapon.GetComponent<MeleeWeapon>().SetAttackDelay(attackDelay);
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);

            //Tracks to the player if they are in range
            if (playerInRange && CanSeePlayer())
            {
                InAttackRange();
            }
        }
    }

    bool CanSeePlayer()
    {
        //Set default output for function
        bool output = false;

        playerDir = GameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        //Raycast to see if enemy can see player
        RaycastHit hit;
        Debug.DrawRay(transform.position, playerDir);
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            //See if enemy can "see" player
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                //Turns towards player when not moving
                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    FaceTarget();
                }

                agent.SetDestination(GameManager.instance.player.transform.position);

                //Return true if player is found properly
                output = true;
            }
        }
        return output;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            //Remove enemy from count on death
            GameManager.instance.UpdateEnemyCount(-1);
            anim.SetBool("Dead", true);
            agent.enabled = false;
            hitBox.enabled = false;
            StopAllCoroutines();
        }
        else
        {
            //Have enemy respond to taking damage from player
            agent.SetDestination(GameManager.instance.player.transform.position);
            FaceTarget();

            //Visual queue for damage taken
            StartCoroutine(FlashDamage());
            anim.SetTrigger("Damage");
        }
    }

    //Creates a red flash to indicate that damage has been taken
    IEnumerator FlashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    //Provides rotation funcionality to track the player
    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
    }

    //Checks if player enters aggression range
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    //Checks if player exits aggression range
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    //Finds if player is within attack range so enemies are not always attacking
    void InAttackRange()
    {
        //If player is in range, sets to true
        if((GameManager.instance.player.transform.position - transform.position).magnitude <= range)
        {
            anim.SetBool("In Attack Range", true);
        }
        else
        {
            anim.SetBool("In Attack Range", false);
        }
    }

    void Die()
    {
        Destroy(gameObject, 2);
    }

    void AttackStart()
    {
        weapon.GetComponent<Collider>().enabled = true;
    }

    void AttackStop()
    {
        weapon.GetComponent<Collider>().enabled = false;
    }
}