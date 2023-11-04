using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerSlide : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float slideDuration = 1.0f;
    [SerializeField] private float slideHeight = 0.5f;
    //[SerializeField] private float slideSpeed = 10.0f;
    
    //camera
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraLoweredPositionY = 0.5f;

    private Vector3 slideDirection;

    private float originalCameraPositionY;
    private float originalHeight;
    private float originalSpeed;
    private bool isSliding;

    void Start()
    {
        originalHeight = controller.height;
        originalCameraPositionY = cameraTransform.localPosition.y;
    }


    public void StartSlide(float currentSpeed, Vector3 playerForward)
    {
        if (isSliding)
            return;

        isSliding = true;
        originalSpeed = currentSpeed;
        slideDirection = playerForward.normalized;

        controller.height *= slideHeight;
        cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, cameraLoweredPositionY, cameraTransform.localPosition.z);

        StartCoroutine(SlideMovement(currentSpeed));
    }

    private IEnumerator SlideMovement(float speed)
    {
        float slideTime = 0f;

        while(slideTime < slideDuration)
        {
            controller.Move(speed * Time.deltaTime * slideDirection);
            
            slideTime += Time.deltaTime;
            yield return null;
        }

        controller.height = originalHeight;
        isSliding = false;
        cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, originalCameraPositionY, cameraTransform.localPosition.z);
    }

    public bool IsSliding()
    {
        return isSliding;
    }

}
