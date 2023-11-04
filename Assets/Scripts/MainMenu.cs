using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
//loads main game scene for play
   public void play()
    {
        SceneManager.LoadScene(1);
    }
//closes the game
    public void exitgame()
    {
        Application.Quit();
    }
}
