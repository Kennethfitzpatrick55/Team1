//using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR;


public class PlayerController : MonoBehaviour, IDamage
{
    // orgization of materials in inspector 
    [Header("----Components----")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;
    [SerializeField] Transform groundRaySource;

    [Header("----Player Stats----")]
    [Range(1, 50)][SerializeField] float Hp;
    [Range(1, 20)][SerializeField] private float playerSpeed;
    [Range(1, 3)][SerializeField] private float sprintMod;
    [Range(1, 3)][SerializeField] int jumpMax;
    [Range(8, 30)][SerializeField] private float jumpHeight;
    //[Range(-10, 40)][SerializeField] private float gravityMod;
    [Range(-10, -40)][SerializeField] private float gravityValue;

    [Header("----Audio----")]
    [SerializeField] AudioClip[] AudDamage;
    [Range(0, 1)][SerializeField] float audDamagevol;
    [SerializeField] AudioClip[] AudJump;
    [Range(0, 1)][SerializeField] float audJumpvol;
    [SerializeField] AudioClip[] AudFootSteps;
    [Range(0, 1)][SerializeField] float audFootStepsvol;

    // private variabels 
    public Vector3 playerVelocity;
    private bool groundedPlayer;
    private Vector3 move;
    private int jumpedtimes;
    bool isSprinting;
    float HPOrig;
    int Layer_Mask;
    bool Crouching;
    bool footstepsPlaying;
    //slide
    private PlayerSlide playerSlide;

    private void Start()
    {
        
        Layer_Mask = LayerMask.GetMask("Wall") + LayerMask.GetMask("Ground");
        HPOrig = Hp;
        spawnPlayer();
    }


    void Update()
    {

        //if its not paused do this 
        if (!GameManager.instance.isPaused)
        {
            CheatsyDoodle();
            
            movement();
            Sprint();
        }
        
    }
    //  controls the players movement 
    void movement()
    {
        //Additnail movemtn called here 
        Sprint();

        //WallRun();

        Crouched();
        //checks to make sure player is grounded
        RaycastHit GroundCheck;
        Debug.DrawRay(groundRaySource.position, transform.TransformDirection(Vector3.down * 0.1f));
        if (groundedPlayer && move.normalized.magnitude > 0.3f && !footstepsPlaying)
        {
            StartCoroutine(PlayFootSteps());
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out GroundCheck, 1.1f, Layer_Mask))
        {
            groundedPlayer = true;
            //sets the players up and down velocity to 0 
            playerVelocity.y = 0f;
            //rests jump to 0 once player lands
            jumpedtimes = 0;
        }
        else
        {
            groundedPlayer = false;
        }

        //vector 2 that recives are players input and moves it to that  postion 
        move = (Input.GetAxis("Horizontal") * transform.right) +
             (Input.GetAxis("Vertical") * transform.forward);

        //syncs are times across computers for performaces 
        controller.Move(move * Time.deltaTime * playerSpeed);

        //will take a button input thats press down 
        if (Input.GetButtonDown("Jump") && jumpedtimes <= jumpMax)
        {
            if (Crouching == true)
            {
                Crouching = false;
                playerSpeed *= sprintMod;
                controller.height *= 2;
            }
            //will assighn are y to some height 
            playerVelocity.y = jumpHeight;
            //play sound
            aud.PlayOneShot(AudJump[Random.Range(0, AudJump.Length)], audJumpvol);
            //and increment jump
            jumpedtimes++;
            //Reset stamina regen?
            GameManager.instance.player.GetComponent<PlayerStamina>().ResetRegen();
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        UpdatePlayerUi();
    }

    IEnumerator PlayFootSteps()
    {
        footstepsPlaying = true;
        aud.PlayOneShot(AudFootSteps[Random.Range(0, AudFootSteps.Length)], audFootStepsvol);

        if (!isSprinting)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
            yield return new WaitForSeconds(0.3f);

        footstepsPlaying = false;
    }

    // addtinal method  for are walk 
    // will incremnet the player speed as
    //as long as they hold the button;
    void Sprint()
    {
        if (Input.GetButtonDown("Sprint") && !isSprinting)
        {
            //if true  increment the player speed by some number 
            playerSpeed *= sprintMod;
            isSprinting = true;
            GameManager.instance.player.GetComponent<PlayerStamina>().ResetRegen();
        }
        else if (Input.GetButtonUp("Sprint") && isSprinting)
        {
            //if false Decrement the player speed 
            playerSpeed /= sprintMod;
            isSprinting = false;
            GameManager.instance.player.GetComponent<PlayerStamina>().ResetRegen();
        }
        //slide check
        if(isSprinting && Input.GetButtonDown("Slide") && groundedPlayer)
        {
            //start slide
            PlayerSlide playerSlideComponent = GetComponent<PlayerSlide>();
            Vector3 slideDirection = transform.forward;

            if(playerSlideComponent != null && !playerSlideComponent.IsSliding())
            {
                playerSlideComponent.StartSlide(playerSpeed, transform.forward);
            }
        }
    }

    public void DisableSprint()
    {
        isSprinting = false;
        playerSpeed /= sprintMod;
    }
   
    void Crouched()
    {
        //check if grouded check button if false
        if (groundedPlayer && Input.GetButtonDown("Crouch") && Crouching == false)
        {
            Crouching = true;
            //change local y scale
            controller.height /= 2;
            //decrement speed
            playerSpeed /= sprintMod;

        }//check if grouded check button if true
        else if (groundedPlayer && Input.GetButtonDown("Crouch") && Crouching == true)
        {
            Crouching = false;
            //set height back to normal 
            controller.height *= 2;
            //give player back speed 
            playerSpeed *= sprintMod;
        }
    }
    void UpdatePlayerUi()
    {
        if (GameManager.instance.playerStaminaBar.fillAmount == 1.0f)
        {
            GameManager.instance.playerStaminaBar.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            GameManager.instance.playerStaminaBar.transform.parent.gameObject.SetActive(true);
        }

        GameManager.instance.playerHPBar.fillAmount = (float)Hp / (float)HPOrig;
    }

    public void TakeDamage(int amount)
    {
        StartCoroutine(GameManager.instance.flash());
        Hp -= amount;
        UpdatePlayerUi();
        if (Hp <= 0)
        {
            GameManager.instance.youLose();
        }
    }

    public void AddHealth(float amount)
    {
        Hp +=(HPOrig * amount);
        if (Hp >= HPOrig)
        {
            Hp = HPOrig;
        }
    }

    public void spawnPlayer()
    {
        Hp = HPOrig;
        UpdatePlayerUi();
        controller.enabled = false;
        transform.position = GameManager.instance.playerSpawn.transform.position;
        controller.enabled = true;
    }

    public bool GetGrounded()
    {
        return groundedPlayer;
    }

    public bool IsSprinting()
    {
        return isSprinting;
    }

    //Cheats for devs to play around with
    void CheatsyDoodle()
    {
        if (Input.GetButton("Submit"))
        {
            controller.enabled = false;
            transform.position = GameManager.instance.devCheat.transform.position;
            controller.enabled = true;
        }
        if(Input.GetButton("L"))
        {
            controller.enabled = false;
            transform.position = GameManager.instance.lightningCheat.transform.position;
            controller.enabled = true;
        }
        if(Input.GetButton("F"))
        {
            controller.enabled = false;
            transform.position = GameManager.instance.fireCheat.transform.position;
            controller.enabled = true;
        }
        if(Input.GetButton("T"))
        {
            controller.enabled = false;
            transform.position = GameManager.instance.treasureCheat.transform.position;
            controller.enabled = true;
        }
    }


}
