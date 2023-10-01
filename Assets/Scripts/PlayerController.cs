using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("-----Components-----")]
    //This is the Charecter Controller
    [SerializeField] CharacterController CC;

    [Header("-----Player States-----")]
    //[Range(1, 10)][SerializeField] HP;
    [Range(1, 10)][SerializeField] float PlayerSpeed;
    [Range(8, 30)][SerializeField] float JumpHeight;
    [Range(1, 40)][SerializeField] int JumpMax;
    [Range(-10, -40)][SerializeField] private float Gravity;
    [Range(1, 10)][SerializeField] private float SprintMod;

    //private 
    private Vector3 PlayerVelocity;
    private Vector3 Move;
    //Int
    private int JumpedTimes;
    //int HPOrig;
    //Bool
    private bool PlayerGrounded;
  
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        


    }
    void Movement()
    {
        // addtinal movement 
        Sprint();

        //Jump is next so lets check if the player is grouded first 
        PlayerGrounded = CC.isGrounded;
        if (PlayerGrounded && PlayerVelocity.y < 0)
        {
            //set the player  Velocity to 0 
            PlayerVelocity.y = 0f;
            //reset jump 
            JumpedTimes = 0;
        }

        //Get Input 
        Move = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        //syncs are times across computers for performaces
        CC.Move(PlayerSpeed * Time.deltaTime * Move);

       

        if (Input.GetButtonDown("Jump") && JumpedTimes <= JumpMax)
        {
            PlayerVelocity.y = JumpHeight;
            //Increment Jump
            JumpedTimes++;
        }

        PlayerVelocity.y += Gravity * Time.deltaTime;
        CC.Move(PlayerVelocity * Time.deltaTime);

    }
    void Sprint ()
    {
        //get Input
        if(Input.GetButtonDown("Sprint"))
        { 
            //If true Increment Speed
            PlayerSpeed *= SprintMod;
        }
        else if(Input.GetButtonUp("Sprint"))
        { 
            // If false Decrement speed
            PlayerSpeed /= SprintMod;
        }
    }
    
}
