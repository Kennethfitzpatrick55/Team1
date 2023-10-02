using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //please ignore this 
    /*
     //[Header("-----Components-----")]
     ////This is the Charecter Controller
     //[SerializeField] Rigidbody RB;

     //[Header("-----Player States-----")]
     //[Header("-----Movement-----")]
     //[Range(1, 10)][SerializeField] float PlayerSpeed;

     ////[Range(8, 30)][SerializeField] float JumpHeight;
     ////[Range(1, 40)][SerializeField] int JumpMax;
     ////[Header("Ground Check")]
     ////[SerializeField] float playerHeight;
     ////[SerializeField] LayerMask Ground;
     ////[SerializeField] float Drag;
     ////[Range(-10, -40)][SerializeField] private float Gravity;
     ////[Range(1, 10)][SerializeField] private float SprintMod;

     ////private 
     ////private Vector3 PlayerVelocity;

     ////Int
     ////private int JumpedTimes;
     ////int HPOrig;
     ////Bool
     ////private bool PlayerGrounded;
     //Vector3 MoveDirection;
     //float HorizontalInput;
     //float VerticalInput;
     //public Transform orentation;


     //// Start is called before the first frame update
     //void Start()
     //{
     //    RB = GetComponent<Rigidbody>();
     //    RB.freezeRotation = true;
     //}

     //// Update is called once per frame
     //void Update()
     //{

     //    MyInput();
     //    Movement();


     //}

     //void MyInput()
     //{
     //    HorizontalInput = Input.GetAxis("Horizontal");
     //    VerticalInput = Input.GetAxis("Vertical");

     //}
     //void Movement()
     //{


     //   MoveDirection = orentation.forward * VerticalInput + orentation.right * HorizontalInput;
     //   RB.AddForce(MoveDirection.normalized * PlayerSpeed);
     //    // addtinal movement 
     //    //Sprint();

     //    //Jump is next so lets check if the player is grouded first 
     //    //PlayerGrounded = CC.isGrounded;
     //    //if (PlayerGrounded && PlayerVelocity.y < 0)
     //    //{
     //    //    //set the player  Velocity to 0 
     //    //    PlayerVelocity.y = 0f;
     //    //    //reset jump 
     //    //    JumpedTimes = 0;
     //    //}

     //    //Get Input 
     //    // this took some time But i think im undertstandign how to get the movement
     //    // lets get control of the speed  
     //    // then will focuse on the jump 
     //    //Move = (Input.GetAxisRaw("Horizontal") * transform.right) + (Input.GetAxisRaw("Vertical") * transform.forward);
     //    //RB.AddForce(orentation.forward = Move.normalized * PlayerSpeed );

     //    //PlayerGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * Ground);
     //    //if(PlayerGrounded)
     //    //{
     //    //    RB.drag = Drag;
     //    //}
     //    //else
     //    //{
     //    //    RB.drag = 0;
     //    //}

     //    //if (Input.GetButtonDown("Jump") && JumpedTimes <= JumpMax)
     //    //{
     //    //    PlayerVelocity.y = JumpHeight;
     //    //    //Increment Jump
     //    //    JumpedTimes++;
     //    //}

     //    //PlayerVelocity.y += Gravity * Time.deltaTime;
     //    //CC.Move(PlayerVelocity * Time.deltaTime);

     //}
     //void Sprint ()
     //{
     //    //get Input
     //    //if(Input.GetButtonDown("Sprint"))
     //    //{ 
     //    //    //If true Increment Speed
     //    //    PlayerSpeed *= SprintMod;
     //    //}
     //    //else if(Input.GetButtonUp("Sprint"))
     //    //{ 
     //    //    // If false Decrement speed
     //    //    PlayerSpeed /= SprintMod;
     //    //}
     //}

     //void Crouch()
     //{


     //}

     //void Slide()
     //{

     //}

     //void WallRun()
     //{

     //}
     */
    // orgization of materials in inspector 
    [Header("----Components----")]
    [SerializeField] CharacterController controller;
 

    [Header("----Player States----")]
    [Range(1, 10)][SerializeField] int Hp;
    [Range(1, 10)][SerializeField] private float playerSpeed;
    [Range(1, 3)][SerializeField] private float sprintMod;
    [Range(1, 3)][SerializeField] int jumpMax;
    [Range(8, 30)][SerializeField] private float jumpHeight;
    [Range(-10, 40)][SerializeField] private float gravityMod;
    [Range(-10, -40)][SerializeField] private float gravityValue;

    [Header("----Gun states----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] GameObject cube;
    // private variabels 
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Vector3 move;
    private int jumpedtimes;
    bool isSprinting;
    bool isShooting;
    int HPOrig;
    private void Start()
    {
        //HPOrig = Hp;
        //spawnPlayer();

        //RB = GetComponent<Rigidbody>();
        //RB.freezeRotation = true;
    }


    void Update()
    {
        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist);

        //if (Input.GetButtonDown("Shoot") && !isShooting)
        //{
        //    StartCoroutine(Shoot());
        //}

        movement();

    }
    //  controls the players movement 
    void movement()
    {
        //Additnail movemtn called here 
        Sprint();

        //checks to make sure player is grounded

        if (groundedPlayer && playerVelocity.y < 0)
        {
            //sets the players up and down velocity to 0 
            playerVelocity.y = 0f;
            //rests jump to 0 once player lands
            jumpedtimes = 0;
        }


        //vector 2 that recives are players input and moves it to that  postion 
        move = (Input.GetAxis("Horizontal") * transform.right) +
             (Input.GetAxis("Vertical") * transform.forward);

        //syncs are times across computers for performaces 
        controller.Move(move * Time.deltaTime * playerSpeed);


        //will take a button input thats press down 
        if (Input.GetButtonDown("Jump") && jumpedtimes <= jumpMax)
        {
            GetComponent<Rigidbody>().AddForce(transform.up * jumpHeight);
            //will assighn are y to some height 
            playerVelocity.y = jumpHeight;
            //and increment jump
            jumpedtimes++;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

    }
    
    // addtinal method  for are walk 
    // will incremnet the player speed as
    //as long as they hold the button;
    void Sprint()
    {
        //get an input
        if (Input.GetButtonDown("Sprint"))
        {
            //if true  increment the player speed by some number 
            playerSpeed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            //if false Decrement the player speed 
            playerSpeed /= sprintMod;
        }
    }

     void WallRun ()
    {
        isWall = true;
        RaycastHit hit;

        if (Physics.Raycast)
        {
            //change gravity
            gravityMod = gravityValue;
            //increase speed 
            playerSpeed *= sprintMod;
            //tilt camera 
        }
        else if (hit.gameObject.tag != ("Wall"))
        {
            //put gravity back to normal 
            gravityValue -=gravityValue;
            //reduce speed 
            playerSpeed /= sprintMod;
            //remove camera tilt 
        }
    }
    // to excute a function in intervals



    //IEnumerator Shoot()
    //{
    //    isShooting = true;

    //    RaycastHit hit;

    //    if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
    //    {
    //        //Does he object have acess to IDamage
    //        IDamage damagable = hit.collider.GetComponent<IDamage>();
    //        // if that object is damagebel then damge it  
    //        if (damagable != null)
    //        {
    //            damagable.takeDamage(shootDamage);
    //        }
    //        //Instantiate(cube, hit.point,cube.transform.rotation);
    //    }
    //    // once fired pause 
    //    yield return new WaitForSeconds(shootRate);
    //    //stop shooting 
    //    isShooting = false;
    //}

    //public void takeDamage(int amount)
    //{
    //    Hp -= amount;
    //    UpdatePlayerUI();
    //    if (Hp <= 0)
    //    {
    //        gameManger.instance.youLose();
    //    }
    //}
    //public void spawnPlayer()
    //{
    //    Hp = HPOrig;
    //    UpdatePlayerUI();
    //    controller.enabled = false;
    //    transform.position = gameManger.instance.playerSpawnPos.transform.position;
    //    controller.enabled = true;
    //}

    //void UpdatePlayerUI()
    //{
    //    gameManger.instance.playerHpBar.fillAmount = (float)Hp / HPOrig;
    //}



}
