using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int lockVerMin;
    [SerializeField] int lockVertMax;
    [SerializeField] bool invertY;

    float xRot;
    // Start is called before the first frame update
    void Start()
    {
        //turn cursour off 
        Cursor.visible = false;
        //lock  cursor inside application 
        Cursor.lockState = CursorLockMode.Locked;


    }

    // Update is called once per frame
    void Update()
    {
        // get mouse (Y && X) input
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

        //rotate on the y axis 
        if (invertY)
        {
            xRot += mouseY;
        }
        else
        {
            xRot -= mouseY;
        }


        //clamp the rot on the X-axis 
        xRot = Mathf.Clamp(xRot, lockVerMin, lockVertMax);

        //rortate the camera on the x-axis
        transform.localRotation = Quaternion.Euler(xRot, 0, 0);

        //roatae the player on the Y - axis 
        transform.parent.Rotate(Vector3.up * mouseX);

    }
}
