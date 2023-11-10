using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public int[,] shopItems = new int[5, 5];
    public float coins;
    public Text coinText;


    // Start is called before the first frame update
    void Start()
    {
        coinText.text = "Drachma:" + coins.ToString();

        //ID's
        shopItems[1,1] = 1;
        shopItems[1,2] = 2;
        shopItems[1,3] = 3;
        shopItems[1,4] = 4;
        //Price
        shopItems[2,1] = 10;
        shopItems[2,2] = 20;
        shopItems[2,3] = 30;
        shopItems[2,4] = 40;
        //quantity
        shopItems[3, 1] = 10;
        shopItems[3, 2] = 20;
        shopItems[3, 3] = 30;
        shopItems[3, 4] = 40;

    }

    // Update is called once per frame
    public void Buy()
    {
        GameObject buttonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;

        if (coins >= shopItems[2, buttonRef.GetComponent<infoButton>().itemID])
        {
            coins -= shopItems[2, buttonRef.GetComponent<infoButton>().itemID];
            shopItems[3, buttonRef.GetComponent<infoButton>().itemID]++;
            coinText.text = "Drachma:" + coins.ToString();
            buttonRef.GetComponent<infoButton>().quantityTxt.text = shopItems[3, buttonRef.GetComponent<infoButton>().itemID].ToString();

        }
    }
}
