using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("----- Player -----")]
    public GameObject player;
    public GameObject playerSpawn;

    [Header("----- Enemy -----")]
    public GameObject enemySpawn;

    float timeScaleOrig;
    int enemyCount;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerSpawn = GameObject.FindWithTag("Player Spawn");

        enemySpawn = GameObject.FindWithTag("Enemy Spawn");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Updates enemy count
    public void UpdateEnemyCount(int amount)
    {
        enemyCount += amount;
    }
}
