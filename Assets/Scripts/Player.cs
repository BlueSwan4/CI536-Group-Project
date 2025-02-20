using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BaseUnit
{
    // Start is called before the first frame update
    public PlayerMovement movementScript;

    void Start()
    {
        speed = 10;
        attack = 5;
        health = 20;

        // subscribe to battle events

        GameManager.OnGameStateChanged += OnGameStateChanage;
        BattleManager.BattleStateChange += OnBattleStateChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGameStateChanage(GameState newState)
    {
        switch (newState) 
        {
            case GameState.Wandering:
                // enable movement
                movementScript.enabled = true;
                break;
            case GameState.Fighting:
                movementScript.enabled = false;
                break;
        }
    }

    private void OnBattleStateChanged(BattleState newBattleState)
    {
        switch (newBattleState) 
        {
            case BattleState.StartBattle:
                break;
            case BattleState.Victory:
                break;
            case BattleState.PlayerTurn:
                break;
            case BattleState.EnemyTurn:
                break;
            case BattleState.Defeat:
                break;
            case BattleState.Inactive:
                break;
        }
    }
}
