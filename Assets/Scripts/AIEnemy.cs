using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Controller script for enemy AI
public class AIEnemy : MonoBehaviour
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
    Color colorOrig;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
