using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Controller script for enemy AI
public class AIEnemy : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;

    [Header("----- Stats -----")]
    [Range(1, 25)] [SerializeField] int HP;
    [SerializeField] int turnSpeed;

    [Header("----- Attack Stats -----")]
    [SerializeField] GameObject weapon;
    [SerializeField] float attackDelay;

    bool isAttacking;
    bool playerInRange;
    float deathAnimTime;
    Vector3 playerDir;
    Color colorOrig;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.UpdateEnemyCount(1);
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange)
        {
            playerDir = GameManager.instance.transform.position - transform.position;
            if(agent.remainingDistance < agent.stoppingDistance)
            {
                FaceTarget();
            }
            agent.SetDestination(GameManager.instance.player.transform.position);
            if(!isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        if(HP <= 0)
        {
            GameManager.instance.UpdateEnemyCount(-1);
            Destroy(gameObject, deathAnimTime);
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
    }
}