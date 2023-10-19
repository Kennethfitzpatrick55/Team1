using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMino : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Collider hitBox;

    [Header("----- Stats -----")]
    [Range(1, 25)][SerializeField] int HP;
    [SerializeField] int turnSpeed;
    [Range(1, 180)][SerializeField] int viewAngle;

    [Header("----- Attack Stats -----")]
    [SerializeField] GameObject weapon;
    [SerializeField] int attackDelay;
    [SerializeField] float range;

    float angleToPlayer;
    Vector3 playerDir;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
            Movement();
        }
    }

    //Runs movement for mino
    void Movement()
    {
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

                //Call for attack if player is within view angle
                if (angleToPlayer <= viewAngle)
                {
                    InAttackRange();
                }
            }
            else
            {
                agent.stoppingDistance = 0;
            }
        }
    }

    //Provides rotation funcionality to track the player
    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
    }

    //Finds if player is within attack range so enemies are not always attacking
    void InAttackRange()
    {
        //If player is in range, sets to true
        if ((GameManager.instance.player.transform.position - transform.position).magnitude <= range)
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
        //Drops health pickup
        GameManager.instance.HealthDrop(transform);
        //Remove object
        Destroy(gameObject, 2);
    }

    void AttackStart()
    {
        //Turns collider on for weapon
        weapon.GetComponent<Collider>().enabled = true;
    }

    void AttackStop()
    {
        //Turns collider off for weapon
        weapon.GetComponent<Collider>().enabled = false;
    }
}
