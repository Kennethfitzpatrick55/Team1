using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class AIEnemyPhantom : MonoBehaviour, IDamage
{
    //Controller script for phantom enemy AI
    [Header("----- Components -----")]
    [SerializeField] Renderer model;

    [Header("----- Stats -----")]
    [Range(1, 25)][SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] float turnSpeed;
    [Range(1, 180)][SerializeField] int viewAngle;
    [SerializeField] float stoppingDist;

    bool playerInRange;
    float angleToPlayer;
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
    }

    // Update is called once per frame
    void Update()
    {
        //Tracks to the player if they are in range
        if (playerInRange && CanSeePlayer())
        {

        }
    }

    bool CanSeePlayer()
    {
        //Set default output for function
        bool output = false;

        playerDir = GameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        //See if player is within view angle
        if (angleToPlayer <= viewAngle)
        {
            //Turn to face player
            FaceTarget();

            //Check if player is further than stopping distance
            if (Vector3.Distance(transform.position, GameManager.instance.player.transform.position) > stoppingDist)
            {
                //Move to player, ignoring environment and flying
                transform.position = Vector3.MoveTowards(transform.position, GameManager.instance.player.transform.position, Time.deltaTime * speed);
            }

            //Return true if player is found properly
            output = true;
        }
        return output;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        //Have enemy respond to taking damage from player
        transform.position = Vector3.MoveTowards(transform.position, GameManager.instance.player.transform.position, Time.deltaTime * speed);
        FaceTarget();

        //Visual queue for damage taken
        StartCoroutine(FlashDamage());

        if (HP <= 0)
        {
            //Set time for future death animations
            Destroy(gameObject, deathAnimTime);
        }
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
    }

    //Creates a red flash to indicate that damage has been taken
    IEnumerator FlashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
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
}