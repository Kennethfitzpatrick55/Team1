using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StealthSceneTrans : MonoBehaviour
{
    [SerializeField] GameObject interact;

    bool playerInRange;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetButtonDown("Interact"))
        {
            SceneManager.LoadScene("Stealth");
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
            interact.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interact.SetActive(false);
        }
    }
}
