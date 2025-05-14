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
        // read rigidbody velocity to determine direction
        if (movementScript.enabled && (movementScript.GetXSpeed() != 0 || movementScript.GetYSpeed() != 0))
        {
            //Debug.Log("Player is moving");
            // get direction, priortise in order nesw
            if (movementScript.GetYSpeed() > 0)
                controller.SetTrigger("walking_north");
            else if (movementScript.GetXSpeed() > 0)
                controller.SetTrigger("walking_east");
            else if (movementScript.GetYSpeed() < 0)
                controller.SetTrigger("walking_south");
            else if (movementScript.GetXSpeed() < 0)
                controller.SetTrigger("walking_west");
        }
        else
        {
            if (GameManager.Instance.State == GameState.Wandering)
                return;

            // player isn't moving, go to idle
            if (controller.GetCurrentAnimatorStateInfo(0).IsName("player_walk_north"))
                controller.SetTrigger("idle_north");
            else if (controller.GetCurrentAnimatorStateInfo(0).IsName("player_walk_east"))
                controller.SetTrigger("idle_east");
            else if (controller.GetCurrentAnimatorStateInfo(0).IsName("player_walk_south"))
                controller.SetTrigger("idle_south");
            else if (controller.GetCurrentAnimatorStateInfo(0).IsName("player_walk_west"))
                controller.SetTrigger("idle_west");
        }
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
