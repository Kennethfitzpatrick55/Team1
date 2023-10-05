using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameOver : MonoBehaviour
{
    [Header("----- Components -----")]

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text restartText;

    [Header("----- Bools -----")]

    private bool isGameOver = false;
    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        //Disables the panel if activated

        gameOverPanel.SetActive(false);
        restartText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //the game over automatically activates and is checked with bool so it doesn't repeat over and over

        if (Input.GetKeyDown(KeyCode.G) && !isGameOver)
        {
            isGameOver = true;

            isPaused = !isPaused;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }


        // if the game has ended
        if (isGameOver)
        {
            // if the restart key is pressed
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            // If the Quit key is pressed
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Application.Quit();
            }

        }
    }
}
