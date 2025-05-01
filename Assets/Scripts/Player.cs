using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : BaseUnit
{
    // Start is called before the first frame update
    public PlayerMovement movementScript;
    public PlayerAnimControl animationScript;

    // Only needed by the player for now. If have multiple players might need to create BaseFriendly class
    public int sp;

    public List<SpellDataSO> playerSpells = new();


    void Start()
    {
        speed = 10;
        attack = 5;
        health = 20;
        sp = 10;

        // subscribe to battle events

        GameManager.OnGameStateChanged += OnGameStateChanage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

// Called when the game manager switches states
    private void OnGameStateChanage(GameState newState)
    {
        switch (newState) 
        {
            case GameState.Wandering:
                // enable movement
                movementScript.EnableMovement();
                break;
            default:
                movementScript.DisableMovement();
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

        // set target to the enemy at the selected index
        target = BattleManager.Instance.enemyUnits[enemyIndex];


        // we have selected an enemy
        // play attack anim
        animationScript.ShowPlayerAttack();
        // start attack coroutine or spell depending on chosen move
        if (BattleManager.Instance.activeSpell == -1)
            StartCoroutine("UseBasicAttack");
        else
        {
            Debug.Log("WE LOVE CASTING SPELLS");
            StartCoroutine("CastSpell");
        }
            
    }

    public IEnumerator CastSpell()
    {
        int multiplier = 1;
        int spellIndex = BattleManager.Instance.activeSpell;
        yield return new WaitForSeconds(attackDuration);
        if (playerSpells[spellIndex].target == SpellDataSO.targetType.single)
        {
            if (target.typeWeaknesses.Contains(playerSpells[spellIndex].element)) multiplier = 2;
            target.ReceieveDamage(playerSpells[spellIndex].baseDamage * multiplier);
        }  
        else
        {
            Debug.Log("Enemies to target: " + BattleManager.Instance.enemyUnits.Count);
            // go through all enemy objects and call receive damage
            for (int targetIndex = 0; targetIndex < BattleManager.Instance.enemyUnits.Count; targetIndex++)
            {
                if (target.typeWeaknesses.Contains(playerSpells[spellIndex].element)) multiplier = 2;
                else multiplier = 1;
                    Debug.Log("Targeting enemy at index: " + targetIndex);
                BattleManager.Instance.enemyUnits[targetIndex].ReceieveDamage(playerSpells[spellIndex].baseDamage * multiplier);
            }
        }

        // remove sp from player
        sp -= playerSpells[spellIndex].spCost;

        // raise turn end event
        target = null;
        Debug.Log("Ending player turn");
        EndUnitTurn();
    }
}
