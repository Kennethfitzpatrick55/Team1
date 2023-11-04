using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopping : MonoBehaviour
{
    public static Shopping instance;

    [Header("----- Components -----")]
    [SerializeField] GameObject shopButton;
    [SerializeField] GameObject shopCanv;
    [SerializeField] GameObject shopMenu;
    [SerializeField] GameObject buyMenu;
    [SerializeField] GameObject sellMenu;
    [SerializeField] GameObject currencyMenu;
    [SerializeField] GameObject hintMenu;

    bool playerInRange;
    GameObject activeMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange && Input.GetButtonDown("Interact"))
        {
            shopCanv.SetActive(true);
            activeMenu = shopMenu;
            activeMenu.SetActive(true);
            GameManager.instance.pausedState();
        }

        //Disable interact button if a menu is set and active
        if(activeMenu != null && activeMenu.activeSelf)
        {
            if (shopButton.activeSelf)
            {
                shopButton.SetActive(false);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger)
        {
            return;
        }

        if(other.CompareTag("Player"))
        {
            playerInRange = true;
            shopButton.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.isTrigger)
        {
            return;
        }

        if(other.CompareTag("Player"))
        {
            playerInRange = false;
            shopButton.SetActive(false);
        }
    }

    public void BuyMenu()
    {
        MenuChange(buyMenu);
    }

    public void SellMenu()
    {
        MenuChange(sellMenu);
    }

    public void EarningMenu()
    {
        MenuChange(currencyMenu);
    }

    public void HintMenu()
    {
        MenuChange(hintMenu);
    }

    //Change from menu to menu
    public void MenuChange(GameObject menu)
    {
        activeMenu.SetActive(false);
        activeMenu = menu;
        activeMenu.SetActive(true);
    }

    //Close all menus
    public void Leave()
    {
        activeMenu.SetActive(false);
        shopCanv.SetActive(false);
        GameManager.instance.unpausedState();
    }

    //Go back to main shop
    public void Back()
    {
        MenuChange(shopMenu);
    }
}
