using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("-----Components-----")]
    //This is the Charecter Controller
    [SerializeField] Rigidbody CC;

    [Header("-----Player States-----")]
    [Header("-----Movement-----")]
    [Range(1, 10)][SerializeField] float PlayerSpeed;
    //[Range(8, 30)][SerializeField] float JumpHeight;
    //[Range(1, 40)][SerializeField] int JumpMax;
    [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask Ground;
    [SerializeField] float Drag;
    //[Range(-10, -40)][SerializeField] private float Gravity;
    //[Range(1, 10)][SerializeField] private float SprintMod;

    //private 
    //private Vector3 PlayerVelocity;
    private Vector3 Move;
    //Int
    private int JumpedTimes;
    //int HPOrig;
    //Bool
    private bool PlayerGrounded;
    public Transform orentation;


    // Start is called before the first frame update
    void Start()
    {
        CC = GetComponent<Rigidbody>();
        CC.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
       
        Movement();
        


    }
    void Movement()
    {
        // addtinal movement 
        //Sprint();

        //Jump is next so lets check if the player is grouded first 
        //PlayerGrounded = CC.isGrounded;
        //if (PlayerGrounded && PlayerVelocity.y < 0)
        //{
        //    //set the player  Velocity to 0 
        //    PlayerVelocity.y = 0f;
        //    //reset jump 
        //    JumpedTimes = 0;
        //}

        //Get Input 
        // this took some time But i think im undertstandign how to get the movement
        // lets get control of the speed  
        // then will focuse on the jump 
        Move = (Input.GetAxisRaw("Horizontal") * transform.right) + (Input.GetAxisRaw("Vertical") * transform.forward);
        CC.AddForce(orentation.forward = Move.normalized * PlayerSpeed );

        PlayerGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * Ground);
        if(PlayerGrounded)
        {
            CC.drag = Drag;
        }
        else
        {
            CC.drag = 0;
        }
        //if (Input.GetButtonDown("Jump") && JumpedTimes <= JumpMax)
        //{
        //    PlayerVelocity.y = JumpHeight;
        //    //Increment Jump
        //    JumpedTimes++;
        //}

        //PlayerVelocity.y += Gravity * Time.deltaTime;
        //CC.Move(PlayerVelocity * Time.deltaTime);

    }
    void Sprint ()
    {
        //get Input
        //if(Input.GetButtonDown("Sprint"))
        //{ 
        //    //If true Increment Speed
        //    PlayerSpeed *= SprintMod;
        //}
        //else if(Input.GetButtonUp("Sprint"))
        //{ 
        //    // If false Decrement speed
        //    PlayerSpeed /= SprintMod;
        //}
    }
    
    void Crouch()
    {
        

    }

    void Slide()
    {

    }

    void WallRun()
    {

    }
}
