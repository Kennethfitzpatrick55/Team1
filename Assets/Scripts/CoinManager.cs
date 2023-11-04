using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public const string Coins = "Drachma";
    public static int coins = 0;

    // Start is called before the first frame update
    void Start()
    {
        coins = PlayerPrefs.GetInt(Coins);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void updateCoins()
    {
        PlayerPrefs.SetInt("Drachma", coins);
        coins = PlayerPrefs.GetInt("Drachma");
        PlayerPrefs.Save();
    }
}
