using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newBehaviorScript : MonoBehaviour
{
    [SerializeField] List<GameObject> objectList = new List<GameObject>();
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int maxObjectsToSpawn;
    [SerializeField] int timeBetweenSpawn;
    [SerializeField] Transform[] spawnPos;

    int curObjectsSpawned;
    bool isSpawning;
    bool startSpawning;

    // Start is called before the first frame update
    void Start()
    {
        //GameManager.instance.updateGameGoal(maxObjectsToSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawning && curObjectsSpawned < maxObjectsToSpawn)
        {
            StartCoroutine(spawn());
        }
    }

    IEnumerator spawn()
    {
        if (!isSpawning)
        {
            isSpawning = true;

            curObjectsSpawned++;
            GameObject objectClone = Instantiate(objectToSpawn, spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
            objectList.Add(objectClone);
            yield return new WaitForSeconds(timeBetweenSpawn);
            isSpawning = false;
        }
    }

    public void updateObjectNum()
    {
        curObjectsSpawned--;
        maxObjectsToSpawn--;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = false;

            for (int i = 0; i < objectList.Count; i++)
            {
                Destroy(objectList[i]);
            }
            objectList.Clear();

            curObjectsSpawned = 0;
        }
    }
}
