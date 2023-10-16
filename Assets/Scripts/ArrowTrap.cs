using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    public class ArrowTrap : MonoBehaviour
    {
        public GameObject arrowPrefab;
        public Transform spawnPoint;
        private float arrowSpeed = 10f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //Debug.Log("Incoming!");
                GameObject arrow = Instantiate(arrowPrefab, spawnPoint.position, spawnPoint.rotation);

                Rigidbody arrowRigidboy = arrow.GetComponent<Rigidbody>();

                if (arrowRigidboy != null)
                {
                    arrowRigidboy.velocity = arrow.transform.right * -1 * arrowSpeed;
                }
                Destroy(arrow, 3f);
            }
        }

        private void SpawnArrow()
        {
            if (arrowPrefab != null)
            {
                //create arrow
                Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                //spawn arrow
                arrowPrefab.SetActive(true);


            }
        }

        private IEnumerator DeactivateArrow(float delay)
        {
            yield return new WaitForSeconds(delay);
            //deactivate arrow
            if (arrowPrefab != null && arrowPrefab.activeSelf)
            {
                arrowPrefab.SetActive(false);
            }
        }

    }
}
