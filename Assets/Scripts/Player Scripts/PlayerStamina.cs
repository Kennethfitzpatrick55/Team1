using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("----Player Stats----")]
    [Range(0f, 100.0f)][SerializeField] float stamina;
    [Range(1, 20)][SerializeField] float jumpCost;
    [Range(1.0f, 100f)][SerializeField] float staminaDrain;
    [Range(1.0f, 100f)][SerializeField] float staminaRegen;
    [Range(1, 100)][SerializeField] float staminaSprintMinimum;
    [Range(1, 100)][SerializeField] float staminaJumpMinimum;
    [Range(1, 10)][SerializeField] float timeUntilRegenStamina;

    float staminaOrig;
    float regenElapsed;
    bool doStaminaRegen;

    // Start is called before the first frame update
    void Start()
    {
        regenElapsed = 0;
        doStaminaRegen = false;
        staminaOrig = stamina;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isPaused)
        {
            CountRegenElapsedInSeconds();
            UpdateStamUI();
            UpdatePlayerStamRegen();
        }
    }

    public void ResetRegen()
    {
        regenElapsed = 0;
    }

    void CountRegenElapsedInSeconds()
    {
        if (regenElapsed < timeUntilRegenStamina)
        {
            regenElapsed += Time.deltaTime;
            doStaminaRegen = false;
        }
        else
        {
            doStaminaRegen = true;
        }
    }

    void UpdateStamUI()
    {
        if (GameManager.instance.playerScript.IsSprinting())
        {
            stamina -= (staminaDrain * Time.deltaTime);
            if (stamina < staminaSprintMinimum)
            {
                GameManager.instance.playerScript.DisableSprint();
                ResetRegen();
            }

            //take stamin away when running 
            GameManager.instance.playerStaminaBar.fillAmount = ((float)stamina / (float)staminaOrig);
        }
        else if (Input.GetButtonDown("Jump") && staminaJumpMinimum < stamina)
        {
            stamina -= jumpCost;

            GameManager.instance.playerStaminaBar.fillAmount = (stamina) / (float)staminaOrig;
        }

        //Prevent stamina from reaching disallowed values
        if (stamina < 0)
        {
            stamina = 0;
        }
    }

    private void UpdatePlayerStamRegen()
    {
        if (!GameManager.instance.playerScript.IsSprinting() && doStaminaRegen)
        {
            stamina += (staminaRegen * Time.deltaTime);
            if (stamina > 100)
            {
                stamina = 100;
            }
            GameManager.instance.playerStaminaBar.fillAmount = (stamina / staminaOrig);
        }
    }
}
