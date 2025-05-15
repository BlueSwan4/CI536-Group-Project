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
        float vert = Input.GetAxis("Vertical");
        float hori = Input.GetAxis("Horizontal");

        if (vert > 0)
            controller.SetTrigger("walking_north");
        else if (vert < 0)
            controller.SetTrigger("walking_south");
        else if (hori > 0)
            controller.SetTrigger("walking_east");
        else if (hori < 0)
            controller.SetTrigger("walking_west");
        else
        {
            // pause current anim clip
            // based on solution by kdgalla (2023)
            controller.SetTrigger("idle");
        }
    }

    public void OnBattleUpdate(BattleState newState)
    {
        Debug.Log("New battle state (from battle manager): " + newState.ToString());
        if (newState != BattleState.Inactive)
            controller.SetBool("in_battle", true);
        else
        {
            Debug.Log("battle end detected");
            controller.SetBool("in_battle", false);
        }
            
    }

    public void ShowPlayerAttack()
    {
        controller.SetTrigger("attacking");
    }
}
