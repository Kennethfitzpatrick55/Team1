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


    [Header("----Player States----")]
    [Range(1, 10)][SerializeField] float Hp;
    [Range(1, 100)][SerializeField] float hpRegen;
    [Range(1, 20)][SerializeField] private float playerSpeed;
    [Range(1, 3)][SerializeField] private float sprintMod;
    [Range(1, 3)][SerializeField] int jumpMax;
    [Range(8, 30)][SerializeField] private float jumpHeight;
    [Range(-10, 40)][SerializeField] private float gravityMod;
    [Range(-10, -40)][SerializeField] private float gravityValue;
    [Range(1, 10)][SerializeField] private float DistanceWall;
    [SerializeField] private Vector3 Crouch;
    [SerializeField] private Vector3 playerScale;
    [Range(1.0f, 100.0f)][SerializeField] float stamina;
    [Range(1, 20)][SerializeField] float jumpCost;
    [Range(1.0f, 100f)][SerializeField] float staminaDrain;
    [Range(1.0f, 100f)][SerializeField] float staminaRegen;
    [Range(1, 100)][SerializeField] float staminaSprintMinimum;
    [Range(1, 100)][SerializeField] float staminaJumpMinimum;
    [Range(1, 10)][SerializeField] float timeUntilRegenHp;
    [Range(1, 10)][SerializeField] float timeUntilRegenStamina;
    [Range(1, 10)][SerializeField] private float WallT;
    //[SerializeField] private float slideT;

    [Header("----Weapon states----")]
    [SerializeField] List<WeaponStats> weaponlist = new List<WeaponStats>();
    [SerializeField] GameObject weaponModel;
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;

    // private variabels 
    public Vector3 playerVelocity;
    private bool groundedPlayer;
    private Vector3 move;
    private int jumpedtimes;
    float staminaOrig;
    bool isSprinting;
    float regenElapsed;
    float hpRegenElapsed;
    bool doStaminaRegen;
    bool doHelthRegen;
    bool isShooting;
    float HPOrig;
    int Layer_Mask;
    bool Crouching;
    bool wallRunning;
    int selectedweapon;
    private Vector3 slide;
    WallRun wallRun;
    private void Start()
    {
        //wallRun = GetComponent<WallRun>();
        regenElapsed = 0;
        doStaminaRegen = false;
        Layer_Mask = LayerMask.GetMask("Wall") + LayerMask.GetMask("Ground");
        HPOrig = Hp;
        staminaOrig = stamina;
        spawnPlayer();
        playerScale = transform.localScale;
        Crouch = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);

    }


    void Update()
    {

        //if its not paused do this 
        if (!GameManager.instance.isPaused)
        {
            if (Input.GetButtonDown("Shoot") && !isShooting)
            {
                StartCoroutine(Shoot());
            }
            
            movement();
            selectGun();
            CountRegenElapsedInSeconds();
        }
    }
    //  controls the players movement 
    void movement()
    {
        //Additnail movemtn called here 
        Sprint();

        updatePlayerStamRegen();

        //WallRun();

        Crouched();

        updatePlayerStamRegen();
        //checks to make sure player is grounded
        RaycastHit GroundCheck;
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down * 1.1f));

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

        //Resets ground velocities for when not wall running (bug fix)
        //if (!wallRun.IsWallRunning())
        //{
        //    playerVelocity.x = 0f;
        //    playerVelocity.z = 0f;
        //}

        //vector 2 that recives are players input and moves it to that  postion 
        move = (Input.GetAxis("Horizontal") * transform.right) +
             (Input.GetAxis("Vertical") * transform.forward);

        //syncs are times across computers for performaces 
        controller.Move(move * Time.deltaTime * playerSpeed);

        //will take a button input thats press down 
        if (Input.GetButtonDown("Jump") && jumpedtimes <= jumpMax && staminaJumpMinimum < stamina)
        {
            if (Crouching == true)
            {
                Crouching = false;


                playerSpeed *= sprintMod;
            }
            //will assighn are y to some height 
            playerVelocity.y = jumpHeight;
            //and increment jump
            jumpedtimes++;
            regenElapsed = 0.0f;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        updatePlayerUi();
    }

    void CountRegenElapsedInSeconds()
    {
        if (regenElapsed < timeUntilRegenStamina)
        {
            regenElapsed += Time.deltaTime;
            //Debug.Log("Counting... " + regenElapsed.ToString());
            doStaminaRegen = false;
        }
        else
        {
            // Debug.Log("Done let's regen");
            doStaminaRegen = true;
        }
        if (hpRegenElapsed < timeUntilRegenHp)
        {
            hpRegenElapsed += Time.deltaTime;
            //Debug.Log("Counting... " + regenElapsed.ToString());
            doHelthRegen = false;
        }
        else
        {
            // Debug.Log("Done let's regen");
            doHelthRegen = true;
        }
    }

    // addtinal method  for are walk 
    // will incremnet the player speed as
    //as long as they hold the button;
    void Sprint()
    {
        if (Input.GetButtonDown("Sprint") && !isSprinting)
        {

            //if (isSprinting && Input.GetButtonDown("Crouch"))
            //{

            //    StartCoroutine(Slide());
            //}

            //if true  increment the player speed by some number 
            playerSpeed *= sprintMod;
            isSprinting = true;
            regenElapsed = 0.0f;
        }
        else if (Input.GetButtonUp("Sprint") && isSprinting)
        {
            //if false Decrement the player speed 
            playerSpeed /= sprintMod;
            isSprinting = false;
            regenElapsed = 0.0f;
        }
    }
    //IEnumerator Slide()
    //{
    //    playerSpeed = 10;
    //    yield return new WaitForSeconds(slideT);
    //    playerSpeed /= sprintMod;
    //    Crouching = true; 
    //    transform.localScale = Crouch;


    //}
    void Crouched()
    {
        //check if grouded check button if false
        if (groundedPlayer && Input.GetButtonDown("Crouch") && Crouching == false)
        {
            Crouching = true;
            //change local y
            //transform.localScale = Crouch;
            controller.height = 1;
            //decrement speed
            playerSpeed /= sprintMod;

        }//check if grouded check button if true
        else if (groundedPlayer && Input.GetButtonDown("Crouch") && Crouching == true)
        {
            Crouching = false;
            //set height back to normal 
            controller.height = 2;
            //give player back speed 
            playerSpeed *= sprintMod;
        }
    }
    void updatePlayerUi()
    {
        //GameManager.instance.playerHPBar.fillAmount = (float)Hp / HPOrig;
        if (isSprinting)
        {
            //if true  increment the player speed by some number 
            stamina -= (staminaDrain * Time.deltaTime);
            if (stamina < 25)
            {
                isSprinting = false;
                playerSpeed /= sprintMod;
            }
            if (stamina < 1)
            {
                stamina = 1;
            }
            //take stamin away when running 
            GameManager.instance.playerStaminaBar.fillAmount = ((float)stamina / (float)staminaOrig);
        }
        else if (Input.GetButtonDown("Jump") && staminaJumpMinimum < stamina)
        {
            //if false Decrement the player speed 
            stamina -= jumpCost;
            if (stamina < 1)
            {
                stamina = 1;
            }
            GameManager.instance.playerStaminaBar.fillAmount = (stamina) / (float)staminaOrig;
        }

        if (GameManager.instance.playerStaminaBar.fillAmount == 1.0f)
        {
            //GameManager.instance.playerStaminaBar.enabled = false;
            GameManager.instance.playerStaminaBar.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            GameManager.instance.playerStaminaBar.transform.parent.gameObject.SetActive(true);
        }

        GameManager.instance.playerHPBar.fillAmount = (float)Hp / (float)HPOrig;

        if (GameManager.instance.playerHPBar.fillAmount < (0.5f) && doHelthRegen)
        {
            Hp += hpRegen * Time.deltaTime;
            if (Hp > (HPOrig / 2))
            {
                Hp = (HPOrig / 2);
                doHelthRegen = false;
            }
        }
    }

    private void updatePlayerStamRegen()
    {
        if (!isSprinting && doStaminaRegen)
        {
            stamina += (staminaRegen * Time.deltaTime);
            if (stamina > 100)
            {
                stamina = 100;
            }
            GameManager.instance.playerStaminaBar.fillAmount = (stamina / staminaOrig);
        }
    }
    //IEnumerator WallTime()
    //{
    //    //tilt camera 
    //    wallRunning = true;
    //    gravityValue += gravityMod;
    //    playerSpeed *= sprintMod;
    //    yield return new WaitForSeconds(WallT);
    //    //tilt back
    //    wallRunning = false;
    //    gravityValue -= gravityMod;
    //    playerSpeed /= sprintMod;
    //}

    //void WallRun()
    //{
    //    RaycastHit hit;
    //    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left * DistanceWall));
    //    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right * DistanceWall));
    //    //if player is by wall do something 
    //    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, DistanceWall, Layer_Mask))
    //    {
    //        //check if ur right wall 
    //        if (hit.collider.tag == "Wall" && !wallRunning)
    //        {



    //            StartCoroutine(WallTime());
    //        }
    //    }
    //    else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, DistanceWall, Layer_Mask))
    //    {

    //        //check if ur left wall 
    //        if (hit.collider.tag == "Wall" && !wallRunning)
    //        {

    //            StartCoroutine(WallTime());
    //        }

    //    }
    //    // to excute a function in intervals
    //}


    public void TakeDamage(int amount)
    {
        Hp -= amount;
        hpRegenElapsed = 0;
        updatePlayerUi();
        if (Hp <= 0)
        {
            GameManager.instance.youLose();
        }
    }


    IEnumerator Shoot()
    {
        // check if we have ammo in the currently selected gun
        if (weaponlist[selectedweapon].ammmoCur > 0)
        {
            isShooting = true;

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
            {
                //Does he object have acess to IDamage
                IDamage damagable = hit.collider.GetComponent<IDamage>();
                // if that object is damagebel then damge it  
                if (damagable != null)
                {
                    damagable.TakeDamage(shootDamage);
                }

            }
            weaponlist[selectedweapon].ammmoCur--;
            GameManager.instance.updateAmmoUI(weaponlist[selectedweapon].ammmoCur, weaponlist[selectedweapon].ammmoMax);
            // once fired pause 
            yield return new WaitForSeconds(shootRate);
            //stop shooting 
            isShooting = false;
        }
    }
    public void setWeaponStats(WeaponStats weapon)
    {
        weaponlist.Add(weapon);
        //states
        shootDamage = weapon.shootDamage;
        shootDist = weapon.shootDist;
        shootRate = weapon.shootRate;
        //model
        weaponModel.GetComponent<MeshFilter>().sharedMesh = weapon.model.GetComponent<MeshFilter>().sharedMesh;

        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weapon.model.GetComponent<MeshRenderer>().sharedMaterial;
        selectedweapon = weaponlist.Count - 1;

        //GameManager.instance.updateAmmoUI(weaponlist[selectedweapon].ammmoCur, weaponlist[selectedweapon].ammmoMax);
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedweapon < weaponlist.Count - 1)
        {
            selectedweapon++;
            changeWeapon();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedweapon > 0)
        {
            selectedweapon--;
            changeWeapon();
        }
    }

    void changeWeapon()
    {
        shootDamage = weaponlist[selectedweapon].shootDamage;
        shootDist = weaponlist[selectedweapon].shootDist;
        shootRate = weaponlist[selectedweapon].shootRate;
        //model
        weaponModel.GetComponent<MeshFilter>().sharedMesh = weaponlist[selectedweapon].model.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weaponlist[selectedweapon].model.GetComponent<MeshRenderer>().sharedMaterial;

        //gameManger.instance.updateAmmoUI(weaponlist[selectedweapon].ammmoCur, weaponlist[selectedweapon].ammmoMax);

        isShooting = false;
    }


    public void spawnPlayer()
    {
        Hp = HPOrig;
        updatePlayerUi();
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




}
