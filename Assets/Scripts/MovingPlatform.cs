using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] float speed = 2.0f;
    
    public Transform[] points;
    public int currentPointIndex = 0;
    public bool isActive = false;

    private void Update()
    {
        if (!isActive || points.Length == 0)
            return;
        MoveNext();
    }

    private void MoveNext()
    { 
        Transform targetPoint = points[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if(transform.position == targetPoint.position)
        {
            currentPointIndex++;
            if(currentPointIndex >= points.Length)
            {
                currentPointIndex = 0;
            }
            
        }
    }

    public void ActivatePlatform()
    {
        isActive = true;
    }
}
