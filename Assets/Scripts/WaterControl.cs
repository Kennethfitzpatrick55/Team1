using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterControl : MonoBehaviour
{
    [SerializeField] float riseSpeed = 0.1f;
    [SerializeField] float maxHeight = 10.0f;
    public ChallengeManager challengeManager;

    public bool isRising = false;

    void Update()
    {
        if(isRising && transform.localScale.y < maxHeight)
        {
            transform.localScale += new Vector3(0, riseSpeed * Time.deltaTime, 0);
            transform.position += new Vector3(0, riseSpeed * Time.deltaTime / 2, 0);
            challengeManager.CheckChallengeStatus();

        }
    }

    public void StartRising()
    {
        isRising = true;
    }
}
