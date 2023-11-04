using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIEnemyRanged : MonoBehaviour, IDamage
{
    //Controller script for ranged enemy AI
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Animator anim;
    [SerializeField] Collider hitBox;

    [Header("----- Stats -----")]
    [Range(1, 25)][SerializeField] int HP;
    [SerializeField] int turnSpeed;
    [Range(1, 180)][SerializeField] int viewAngle;
    [SerializeField] int roamDist;
    [SerializeField] int roamTime;

    [Header("----- Attack Stats -----")]
    //Weapon implementation
    [SerializeField] GameObject weapon;
    [SerializeField] Transform weaponPos;
    [SerializeField] float attackDelay;
    [Range(1, 45)][SerializeField] int attackAngle;
    float range;

    bool isAttacking;
    bool playerInRange;
    float angleToPlayer;
    float stoppingDistOrig;
    bool destinationChosen;
    Vector3 playerDir;
    Color colorOrig;
    Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        //Fetches original color to not mess up color/model after damage flash
        colorOrig = model.material.color;

        //Set range of attack based on weapon equipped
        range = weapon.GetComponent<Projectile>().GetRange();

        //Save starting position for roaming
        startingPos = transform.position;

        //Save original stopping distance for manipulation
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);

            //Tracks to the player if they are in range
            if (!playerInRange)
            {
                StartCoroutine(Roam());
            }
            else if (playerInRange && !CanSeePlayer())
            {
                StartCoroutine(Roam());
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
                //Set to regular stopping distance for player tracking
                agent.stoppingDistance = stoppingDistOrig;

                //Turns towards player when not moving
                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    FaceTarget();
                }

                agent.SetDestination(GameManager.instance.player.transform.position);

                //Attacks if not attacking and in attacking range
                if (!isAttacking && angleToPlayer <= attackAngle)
                {
                    StartCoroutine(Attack());
                }

                //Return true if player is found properly
                output = true;
            }
            else
            {
                //Make stopping distance 0 for roaming
                agent.stoppingDistance = 0;
            }
        }
        return output;
    }

    //Allows enemy to have passive actions
    IEnumerator Roam()
    {
        if (agent.remainingDistance < 0.05f && !destinationChosen)
        {
            destinationChosen = true;
            //Changes stopping distance so enemy will hit the chosen destination
            agent.stoppingDistance = 0;

            yield return new WaitForSeconds(roamTime);
            //Picks location within given range
            Vector3 randomPos = UnityEngine.Random.insideUnitSphere * roamDist;
            randomPos += startingPos;

            NavMeshHit hit;
            //Validates location picked
            NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);

            destinationChosen = false;
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
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

    //Runs attacking functionality
    IEnumerator Attack()
    {
        isAttacking = true;

        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDelay);

        isAttacking = false;
    }

    public void CreateAttack()
    {
        Instantiate(weapon, weaponPos.position, transform.rotation);
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

    public void Die()
    {
        //Drops health pickup on death
        GameManager.instance.HealthDrop(transform);
        //Removes object
        Destroy(gameObject, 2);
    }
}