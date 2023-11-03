using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class AIMino : MonoBehaviour, IDamage
{
    enum bossStages
    {
        FIRST_STAGE,
        SECOND_STAGE,
        THIRD_STAGE,
        numberOfStages,
    }

    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Collider hitBox;

    [Header("----- Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int turnSpeed;
    [Range(1, 180)][SerializeField] int viewAngle;

    [Header("----- Attack Stats -----")]
    [SerializeField] GameObject weapon;
    [SerializeField] int attackDelay;
    [SerializeField] float range;

    float currSpeed;
    float scalar;
    float stageTwoSpeed;
    bool isGrowing;
    float angleToPlayer;
    Vector3 playerDir;
    bossStages currentStage;



    // Start is called before the first frame update
    void Start()
    {
        currentStage = bossStages.FIRST_STAGE;
        isGrowing = false;
    }

    // Update is called once per frame
    void Update()
    {
        //debug and testing keys for minatour stages
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            StartStageTwo();
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            StartStageThree();
            
        }
        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
            Movement();
        }
        if (isGrowing)
        {
            scalar += Time.deltaTime;
            agent.speed = 0;
            transform.localScale = new Vector3(Mathf.Lerp(1.5f, 3f, ParametricGrowthCurve(scalar)), Mathf.Lerp(1.5f, 3f, ParametricGrowthCurve(scalar)), Mathf.Lerp(1.5f, 3f, ParametricGrowthCurve(scalar)));
            if (scalar >= 2)
            {
                isGrowing = false;
                agent.speed = stageTwoSpeed;
            }
        }
    }

    //Runs movement for mino
    void Movement()
    {
        playerDir = GameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

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
        else
        {
            anim.SetInteger("In Attack Range", 0);
        }
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
            anim.SetTrigger("Damage");
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
            int animNumber = Random.Range(1, 4);
            Debug.Log(animNumber);
            anim.SetInteger("In Attack Range", animNumber);
        }
        else
        {
            anim.SetInteger("In Attack Range", 0);
        }
    }

    void Die()
    {
        //Drops health pickup
        GameManager.instance.youWin();
        //Remove object
        Destroy(gameObject, 2);
    }

    void AttackStart()
    {
        //Turns collider on for weapon
        weapon.GetComponent<Collider>().enabled = true;
        currSpeed = agent.speed;
        agent.speed = 0;
    }

    void BossStageChange(bossStages bStage)
    {
        switch (bStage)
        {
            case bossStages.SECOND_STAGE:
                StartStageTwo();
                break;
            case bossStages.THIRD_STAGE:
                StartStageThree();
                break;
        }
    }

    void AttackStop()
    {
        //Turns collider off for weapon
        weapon.GetComponent<Collider>().enabled = false;
        agent.speed = currSpeed;
    }

    void StartStageTwo()
    {
        agent.speed *= 1.5f;
        stageTwoSpeed = agent.speed;
        isGrowing = true;
        scalar = 0;
        anim.SetTrigger("Roar Trigger");
        agent.stoppingDistance = 6;
        range *= 2;
    }
    void StartStageThree()
    {

    }

    void ChargeAttack()
    {

    }
    float ParametricGrowthCurve(float t)
    {
        t /= 2;
        float sqt = t * t;
        return sqt / (2.0f * (sqt - t) + 1.0f);
    }
}
