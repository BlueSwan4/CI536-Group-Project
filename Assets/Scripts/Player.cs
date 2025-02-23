using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BaseUnit
{
    // Start is called before the first frame update
    public PlayerMovement movementScript;
    public PlayerAnimControl animationScript;

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
                movementScript.EnableMovement();
                break;
            case GameState.Fighting:
                movementScript.DisableMovement();
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

    public void OnEnemySelected(int enemyIndex)
    {
        // return if we are not in a battle
        if (GameManager.Instance.State != GameState.Fighting)
            return;
        // if there aren't enemies left, also return
        if (!(BattleManager.Instance.enemyUnits.Count > 0))
            return;

        // we have selected an enemy
        // play attack anim
        animationScript.ShowPlayerAttack();
        // start attack coroutine
        StartCoroutine("UseBasicAttack");
    }
}
