using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonFunctions : MonoBehaviour
{
    public void resume()
    {
        GameManager.instance.unpausedState();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.unpausedState();
    }

    public void quit()
    {
        Application.Quit();
    }

    //public void respawnPlayer()
    //{
    //    GameManager.instance.unpausedState();
    //    GameManager.instance.playerScript.spawnPlayer();
    //}
}
