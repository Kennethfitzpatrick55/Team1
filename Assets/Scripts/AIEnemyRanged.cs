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

    [Header("----- Stats -----")]
    [Range(1, 25)] [SerializeField] int HP;
    [SerializeField] int turnSpeed;
    [Range(1, 180)] [SerializeField] int viewAngle;

    [Header("----- Attack Stats -----")]
    //Weapon implementation
    [SerializeField] GameObject weapon;
    [SerializeField] Transform weaponPos;
    [SerializeField] float attackDelay;
    [Range(1, 45)] [SerializeField] int attackAngle;
    float range;

    bool isAttacking;
    bool playerInRange;
    float angleToPlayer;
    float deathAnimTime;
    Vector3 playerDir;
    Color colorOrig;

    // Start is called before the first frame update
    void Start()
    {
        //FOR FUTURE USE, sets timer to destroy an enemy to allow for death animations to play (defaulting 0 for now)
        deathAnimTime = 0;

        //Fetches original color to not mess up color/model after damage flash
        colorOrig = model.material.color;

        //Set range of attack based on weapon equipped
        range = weapon.GetComponent<Projectile>().GetRange();

        //Update enemy count with each spawn
        GameManager.instance.UpdateEnemyCount(1);
    }

    // Update is called once per frame
    void Update()
    {
        //Resets enemy tracking if the player has respawned (fixes respawn not triggering exit)
        RespawnReset();

        //Tracks to the player if they are in range
        if(playerInRange && CanSeePlayer())
        {
            
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

                //Attacks if not attacking and in attacking range
                if (!isAttacking && angleToPlayer <= attackAngle)
                {
                    StartCoroutine(Attack());
                }

                //Return true if player is found properly
                output = true;
            }
        }
        return output;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        //Have enemy respond to taking damage from player
        agent.SetDestination(GameManager.instance.player.transform.position);
        FaceTarget();

        //Visual queue for damage taken
        StartCoroutine(FlashDamage());

        if(HP <= 0)
        {
            //Remove enemy from count on death
            GameManager.instance.UpdateEnemyCount(-1);

            //Set time for future death animations
            Destroy(gameObject, deathAnimTime);
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

        //Creates weapon to use
        Instantiate(weapon, weaponPos.position, transform.rotation);

        yield return new WaitForSeconds(attackDelay);

        isAttacking = false;
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

    //Fix for bug of player respawning not exit triggering enemy tracking
    void RespawnReset()
    {
        //Checks if player respawned, then resets the playerInRange variable
        //if (GameManager.instance.player.GetComponent<PlayerController>().DidRespawn())
        //{
        //    playerInRange = false;
        //}
    }
}