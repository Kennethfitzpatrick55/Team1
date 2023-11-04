using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
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
    [SerializeField] GameObject chargeColliderObject;
    [SerializeField] int attackDelay;
    [SerializeField] float range;
    [SerializeField] float rushRange;
    [SerializeField] float chargeAttackCooldown;

    float currSpeed;
    float scalar;
    float stageTwoSpeed;
    bool isGrowing;
    bool isChargeAttacking;
    float chargeAttackCooldownTracker;

    float angleToPlayer;
    Vector3 playerDir;
    Vector3 chargeDir;
    Vector3 chargedestination;
    bossStages currentStage;



    // Start is called before the first frame update
    void Start()
    {
        currentStage = bossStages.FIRST_STAGE;
        isGrowing = false;
        isChargeAttacking = false;
        chargeAttackCooldownTracker = 0;
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
            //Scales boss size via Parametric growth curve noramlized for adjustments
            scalar += Time.deltaTime;
            agent.speed = 0;
            transform.localScale = new Vector3(Mathf.Lerp(1.5f, 3f, ParametricGrowthCurve(scalar)), Mathf.Lerp(1.5f, 3f, ParametricGrowthCurve(scalar)), Mathf.Lerp(1.5f, 3f, ParametricGrowthCurve(scalar)));
            if (scalar >= 2)
            {
                isGrowing = false;
                agent.speed = stageTwoSpeed;
            }
        }
        if(currentStage == bossStages.SECOND_STAGE)
        {
            chargeAttackCooldownTracker = Mathf.Max(chargeAttackCooldownTracker - Time.deltaTime, 0);
        }
    }

    //Runs movement for mino
    void Movement()
    {
        Debug.Log("Movement()");
        if(isChargeAttacking)
        {
            Debug.Log("AHAHAHAHAHAHA RUSHING IN!!!");
            RaycastHit hit;            
            if (Physics.Raycast(transform.position, chargeDir, out hit, 2, 6))
            {
                onHitWall();
                return;
            }
            if(agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                onEndCharge();
            }
            return;
        }

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
            anim.SetInteger("In Attack Range", animNumber);
            return;
            // we want to return here so that we escape the rush attack when within range for regular attacks
        }
        else
        {
            anim.SetInteger("In Attack Range", 0);
        }

        if(currentStage == bossStages.SECOND_STAGE && chargeAttackCooldownTracker <= 0)
        {
            Debug.Log("Okay we made it. Now next, range test");
            if ((GameManager.instance.player.transform.position - transform.position).magnitude <= rushRange)
            {
                ChargeAttack();
            }
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
    //Establishes size/speed gain for stage 2, adjusts in range distacne to compensate
    void StartStageTwo()
    {
        currentStage = bossStages.SECOND_STAGE;
        agent.speed *= 1.5f;
        stageTwoSpeed = agent.speed;
        isGrowing = true;
        scalar = 0;
        anim.SetTrigger("Roar Trigger");
        agent.stoppingDistance = 6;
        range *= 2;
        rushRange *= 2;
        chargeAttackCooldownTracker = 10;
    }
    void StartStageThree()
    {
        currentStage = bossStages.THIRD_STAGE;
    }

    void ChargeAttack()
    {
        // save player's direction here to charge
        chargeAttackCooldownTracker = chargeAttackCooldown;

        anim.SetTrigger("Charge Attack Trigger");
        Debug.Log("Roar!!!! Start charge!");
    }

    void TriggerStartRush()
    {
        Debug.Log("RUSH TIME!!!");
        chargeDir = GameManager.instance.player.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(chargeDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
        isChargeAttacking = true;
        agent.speed = 12;
        chargeColliderObject.GetComponent<Collider>().enabled = true;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, rot.ToEulerAngles(), out hit, 35, 6))
        {
            chargedestination = hit.point;
            agent.SetDestination(hit.point);
        }
        else
        {
            chargedestination = transform.position + (chargeDir.normalized * (35));
            agent.SetDestination(chargedestination);
        }
    }

    void onHitWall()
    {
        Debug.Log("Ouch I hit a wall");
        // call this when the minotaur collides with a wall
        agent.speed = 0;
        anim.SetTrigger("Charge Hit Wall Trigger");
        isChargeAttacking = false;
        chargeColliderObject.GetComponent<Collider>().enabled = false;
    }

    void onEndCharge()
    {
        anim.SetTrigger("Charge End No Hit");
        isChargeAttacking = false;
        chargeColliderObject.GetComponent<Collider>().enabled = false;
        EndChargeAttack();
    }

    void EndChargeAttack()
    {
        Debug.Log("back to normal now");
        agent.speed = stageTwoSpeed;
    }

    float ParametricGrowthCurve(float t)
    {
        t /= 2;
        float sqt = t * t;
        return sqt / (2.0f * (sqt - t) + 1.0f);
    }
}
