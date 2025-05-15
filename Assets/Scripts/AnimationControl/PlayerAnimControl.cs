using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimControl : MonoBehaviour
{
    public Animator controller;
    public PlayerMovement movementScript;

    void Start()
    {
        if (movementScript == null)
        {
            movementScript = GetComponent<PlayerMovement>();
        }

        // subscribe to battle state update event
        BattleManager.BattleStateChange += OnBattleUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        // determine walking direction
        if (movementScript.enabled && movementScript.canMove)
        {
            if (movementScript.GetYSpeed() > 0)
            {
                controller.SetBool("moving_north", true);
                controller.SetBool("moving_east", false);
                controller.SetBool("moving_south", false);
                controller.SetBool("moving_south", false);
            }
            else if (movementScript.GetXSpeed() > 0)
            {
                controller.SetBool("moving_north", false);
                controller.SetBool("moving_east", false);
                controller.SetBool("moving_south", false);
                controller.SetBool("moving_west", false);
            }
            else if (movementScript.GetYSpeed() < 0)
            {
                controller.SetBool("moving_north", false);
                controller.SetBool("moving_east", false);
                controller.SetBool("moving_south", true);
                controller.SetBool("moving_west", false);
            }
            else if (movementScript.GetXSpeed() < 0)
            {
                controller.SetBool("moving_north", false);
                controller.SetBool("moving_east", false);
                controller.SetBool("moving_south", false);
                controller.SetBool("moving_west", true);
            }
            else
            {
                // set all to false
                controller.SetBool("moving_north", false);
                controller.SetBool("moving_east", false);
                controller.SetBool("moving_south", false);
                controller.SetBool("moving_west", false);
            }
        }
        controller.SetBool("moving_north", false);
        controller.SetBool("moving_east", false);
        controller.SetBool("moving_south", false);
        controller.SetBool("moving_west", false);
    }

    public void OnBattleUpdate(BattleState newState)
    {
        Debug.Log("New battle state (from battle manager): " + newState.ToString());
        if (newState != BattleState.Inactive)
            controller.SetBool("battle_running", true);
        else
        {
            Debug.Log("battle end detected");
            controller.SetBool("battle_running", false);
        }
            
    }

    public void ShowPlayerAttack()
    {
        controller.SetTrigger("attacking");
    }
}
