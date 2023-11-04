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
    [Range(1, 100)][SerializeField] float wallDetectionDistance;
    [Range(1, 100)][SerializeField] float timeSpentCharging;
    [SerializeField] List<GameObject> enemySpawners;
    [SerializeField] GameObject enemyPrefab;


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
    //represents in seconds
    [Range(1, 30)][SerializeField] float chargeAttackCooldown;
    [Range(1, 30)][SerializeField] float summonCooldown;

    float currSpeed;
    float scalar;
    float stageTwoSpeed;
    bool isGrowing;
    bool isShrinking;
    bool isChargeAttacking;
    float chargeAttackCooldownTracker;
    float summonCooldownTracker;

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
        summonCooldownTracker = 0;
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
        if (isShrinking)
        {
            //Scales boss size via Parametric growth curve noramlized for adjustments
            scalar += Time.deltaTime;
            agent.speed = 0;
            transform.localScale = new Vector3(Mathf.Lerp(3f, 1.5f, ParametricGrowthCurve(scalar)), Mathf.Lerp(3f, 1.5f, ParametricGrowthCurve(scalar)), Mathf.Lerp(3f, 1.5f, ParametricGrowthCurve(scalar)));
            if (scalar >= 2)
            {
                isShrinking = false;
                agent.speed = stageTwoSpeed;
            }
        }
        if (currentStage == bossStages.SECOND_STAGE)
        {
            //compares time passed to cooldown time
            chargeAttackCooldownTracker = Mathf.Max(chargeAttackCooldownTracker - Time.deltaTime, 0);
        }
        else if (currentStage == bossStages.THIRD_STAGE)
        {
            summonCooldownTracker = Mathf.Max(summonCooldownTracker - Time.deltaTime, 0);
            SummonAllies();
        }
    }
    void Movement()
    {
        
        if(isChargeAttacking)
        {
            timeSpentCharging -= Time.deltaTime;
            if (timeSpentCharging <= 0)
            {
                onEndCharge();
                return;
            }
            Debug.Log("AHAHAHAHAHAHA RUSHING IN!!!");
            agent.SetDestination(chargedestination);
            RaycastHit hit;
            Debug.DrawLine(hitBox.transform.position, transform.forward);
            if (Physics.Raycast(hitBox.transform.position, transform.forward, out hit))
            {
                if (hit.collider.CompareTag("Wall") && hit.distance <= wallDetectionDistance)
                {
                    Debug.Log("Hit wall at: " + transform.position);
                    onHitWall();
                    return;
                }
            }
            if(agent.remainingDistance == 0)
            {
                Debug.Log("End charge at: " + transform.position);
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
        chargeAttackCooldownTracker = 5;
    }
    void StartStageThree()
    {
        currentStage = bossStages.THIRD_STAGE;
        summonCooldownTracker = 5;
        isShrinking = true;
        scalar = 0;
        agent.stoppingDistance = 3;
        range /= 2;
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

        // flatten direction to one world axis
        if (Mathf.Abs(chargeDir.x) > Mathf.Abs(chargeDir.z))
            chargeDir.z = 0;
        else
            chargeDir.x = 0;
        chargeDir.y = 0;

        chargeDir = chargeDir.normalized;

        Quaternion rot = Quaternion.LookRotation(chargeDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
        timeSpentCharging = 5;

        isChargeAttacking = true;
        agent.speed = 25;
        chargeColliderObject.GetComponent<Collider>().enabled = true;

        Ray ray = new Ray();
        ray.origin = transform.position;
        ray.direction = chargeDir;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1500, 6))
        {
            Debug.Log("Hit something, charge point: " + hit.point);
            chargedestination = hit.point;
            agent.SetDestination(hit.point);
        }
        else
        {
            chargedestination = transform.position + (chargeDir * (1500));
            Debug.Log("From: " +transform.position + " Charge Point: " + chargedestination);
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
        isChargeAttacking = false;
        chargeAttackCooldownTracker = chargeAttackCooldown;
        agent.speed = stageTwoSpeed;
        agent.SetDestination(GameManager.instance.player.transform.position);
    }

    void SummonAllies()
    {
        agent.speed = 0;
        if (summonCooldownTracker <= 0)
        {
            for (int i = 0; i < enemySpawners.Count; i++)
            {
                Instantiate(enemyPrefab, enemySpawners[i].transform.position, enemySpawners[i].transform.rotation);
            }
            summonCooldownTracker = summonCooldown;
        }
        agent.speed = stageTwoSpeed;
    }

    float ParametricGrowthCurve(float t)
    {
        t /= 2;
        float sqt = t * t;
        return sqt / (2.0f * (sqt - t) + 1.0f);
    }
}
