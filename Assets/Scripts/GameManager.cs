using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEditor.ProBuilder;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("----- Player -----")]
    public GameObject player;
    public GameObject playerSpawn;
    public Image playerHPBar;
    public Image playerStaminaBar;
    public PlayerController playerScript;

    [Header("----- Enemy -----")]
    public GameObject enemySpawn;
    [SerializeField] int rangedEnemiesMax;
    [SerializeField] int meleeEnemiesMax;
    [SerializeField] int phantomsMax;
    [SerializeField] GameObject HPPickup;

    [Header("----Menus----")]
    [SerializeField] GameObject activeMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject winMenu;
    [SerializeField] GameObject lossMenu;
    [SerializeField] TextMeshProUGUI ammoCur;
    [SerializeField] TextMeshProUGUI ammoMax;
    [SerializeField] GameObject PlayerDamageFlashScreen;

    public bool isPaused;
    float timeScaleOrig;
    int enemyCount;

    //Only uncomment code once implemented
    void Awake()
    {
        
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawn = GameObject.FindWithTag("Respawn");
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

    // Pauses the game
    public void pausedState()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        playerHPBar.gameObject.SetActive(false);
        playerStaminaBar.gameObject.SetActive(false);
    }

    //Resumes the game
    public void unpausedState()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        activeMenu.SetActive(false);
        activeMenu = null;
        playerHPBar.gameObject.SetActive(true);
        playerStaminaBar.gameObject.SetActive(true);
    }

    public void youLose()
    {
        lossMenu.SetActive(true);
        activeMenu = lossMenu;
        pausedState();
    }

    public void youWin()
    {
        winMenu.SetActive(true);
        activeMenu = winMenu;
        pausedState();
    }

    public IEnumerator flash()
    {
        PlayerDamageFlashScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        PlayerDamageFlashScreen.SetActive(false);
    }

    public void updateAmmoUI(int cur, int max)
    {
        ammoCur.text = cur.ToString("F0");
        ammoMax.text = max.ToString("F0");
    }

    public int GetMaxRanged()
    {
        return rangedEnemiesMax;
    }

    public int GetMaxMelee()
    {
        return meleeEnemiesMax;
    }

    public int GetMaxPhantom()
    {
        return phantomsMax;
    }

    public void HealthDrop(Transform pos)
    {
        Instantiate(HPPickup, pos.position, Quaternion.identity);
    }
}
