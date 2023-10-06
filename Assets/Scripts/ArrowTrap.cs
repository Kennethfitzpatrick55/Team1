using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    public GameObject arrowPrefab;
    private Coroutine arrowDeactivationCoroutine;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Incoming!");
            SpawnArrow();
        }
    }

    private void SpawnArrow()
    {
        if(arrowPrefab != null)
        {
            //create arrow
            Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            //spawn arrow
            arrowPrefab.SetActive(true);
            
            arrowDeactivationCoroutine = StartCoroutine(DeactivateArrow(6f));
        }
    }

    private IEnumerator DeactivateArrow(float delay)
    {
        yield return new WaitForSeconds(delay);
        //deactivate arrow
        if(arrowPrefab != null && arrowPrefab.activeSelf)
        {
            arrowPrefab.SetActive(false);
        }
    }

    

}
