using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class infoButton : MonoBehaviour
{
    public int itemID;
    public Text priceTxt;
    public Text quantityTxt;
    public GameObject ShopManager;

    // Start is called before the first frame update
    void Start()
    {
        priceTxt.text = "Drachma Required: " + ShopManager.GetComponent<ShopManager>().shopItems[2, itemID].ToString();
        quantityTxt.text = ShopManager.GetComponent<ShopManager>().shopItems[2, itemID].ToString();
    }


}
