using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.ProBuilder;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("----- Player -----")]
    public GameObject player;
    public GameObject playerSpawn;
    public Image playerHPBar;
    public Image playerStaminaBar;

    [Header("----- Enemy -----")]
    public GameObject enemySpawn;

    [Header("----Menus----")]
    [SerializeField] GameObject activeMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject winMenu;
    [SerializeField] GameObject lossMenu;


    public bool isPaused;
    float timeScaleOrig;
    int enemyCount;

    //Only uncomment code once implemented
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        //playerSpawn = GameObject.FindWithTag("Player Spawn");
        //enemySpawn = GameObject.FindWithTag("Enemy Spawn");
    }

    
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && activeMenu == null)
        {
            pausedState();
            activeMenu = pauseMenu;
            activeMenu.SetActive(isPaused);
        }
    }

    //Updates enemy count
    public void UpdateEnemyCount(int amount)
    {
        enemyCount += amount;
    }

    public void pausedState()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void unpausedState()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        activeMenu.SetActive(false);
        activeMenu = null;
    }
}
