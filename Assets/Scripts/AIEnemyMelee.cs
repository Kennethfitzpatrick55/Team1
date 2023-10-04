using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIEnemyMelee : MonoBehaviour, IDamage
{
    //Controller script for melee enemy AI
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;

    [Header("----- Stats -----")]
    [Range(1, 25)] [SerializeField] int HP;
    [SerializeField] int turnSpeed;

    [Header("----- Attack Stats -----")]
    //Sets weapon to use
    [SerializeField] GameObject weapon;
    [SerializeField] Transform weaponPos;
    [SerializeField] float attackDelay;
    float range;

    bool isAttacking;
    bool playerInRange;
    float deathAnimTime;
    Vector3 playerDir;
    Color colorOrig;

    // Start is called before the first frame update
    void Start()
    {
        //FOR FUTURE USE, sets timer to destroy an enemy to allow for death animations to play
        deathAnimTime = 0;

        //Fetches original color to not mess up color/model after damage flash
        colorOrig = model.material.color;

        //Set range of attack based on weapon equipped (weapon position is center of weapon, plus half of the y scale for a downward swing)
        range = Vector3.Distance(weaponPos.position, gameObject.transform.position) + (weapon.transform.localScale.y / 2);

        //Update enemy count with each spawn
        GameManager.instance.UpdateEnemyCount(1);
    }

    // Update is called once per frame
    void Update()
    {
        //Resets enemy tracking if the player has respawned (fixes respawn not triggering exit)
        RespawnReset();

        //Tracks to the player if they are in range
        if (playerInRange)
        {
            playerDir = GameManager.instance.player.transform.position - transform.position;
            
            //Turns towards player when not moving
            if(agent.remainingDistance < agent.stoppingDistance)
            {
                FaceTarget();
            }

            agent.SetDestination(GameManager.instance.player.transform.position);

            //Attacks if not attacking and in attacking range
            if(!isAttacking && InAttackRange())
            {
                StartCoroutine(Attack());
            }

            //Animate weapon if attacking
            //if (isAttacking)
            //{
            //    weapon.GetComponent<MeleeWeapon>().Animate();
            //}
            //else
            //{
            //    weapon.GetComponent<MeleeWeapon>().ResetAnim();
            //}
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

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
        //if(GameManager.instance.player.GetComponent<PlayerController>().DidRespawn())
        //{
        //    playerInRange = false;
        //}
    }

    //Finds if player is within attack range so enemies are not always attacking
    bool InAttackRange()
    {
        //Defaults to not in range
        bool inRange = false;

        //If player is in range, sets to true
        if(Vector3.Distance(GameManager.instance.player.transform.position, weaponPos.transform.position) <= range)
        {
            inRange = true;
        }

        return inRange;
    }
}