using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Give : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Press()
    {
        CoinManager.coins += 50;
        CoinManager.updateCoins();
    }
}
