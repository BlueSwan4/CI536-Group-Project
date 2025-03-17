using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCursor : MonoBehaviour
{
    [Header("GameObject References")]
    public SpriteRenderer cursorSprite;

    [SerializeField]private bool selectingEnemy = false;

    // event for enemy selection - raised when enemy is selected
    public static event Action<int, bool> EnemySelected;
    [SerializeField]private int selectionIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // subscribe to the battle state change event
        BattleManager.BattleStateChange += OnBatleStateChange;
        cursorSprite.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool targetingSingle = true;
        if (selectingEnemy)
        {
            // movement - use wasd for selecting enemies
            // w / up - move cursor up
            // s / down - move cursor down


            // check there is no multi-target spell active


            if (BattleManager.Instance.activeSpell != -1)
            {
                // we have an active spell
                // check if it's multihit
                Player currentPlayer = BattleManager.Instance.battleUnits[BattleManager.Instance.turnIndex] as Player;
                targetingSingle = currentPlayer.playerSpells[BattleManager.Instance.activeSpell].target == SpellDataSO.targetType.single;
            }
            if (targetingSingle)
            {
                if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    // decrement index - if we go under 0 wrap it round to end of enemies
                    selectionIndex--;

                    if (selectionIndex < 0)
                        selectionIndex = BattleManager.Instance.enemyUnits.Count - 1;
                }
                else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
                {
                    // increment index as we go down - if we go over the limit wrap to 0
                    selectionIndex++;

                    if (selectionIndex >= BattleManager.Instance.enemyUnits.Count)
                        selectionIndex = 0;
                }
            }

            // move cursor to chosen enemy pos
            transform.position = BattleManager.Instance.enemyUnits[selectionIndex].transform.position + new Vector3(-1, 0, 0);

            // check for selection input (z key)
            if (Input.GetKeyUp(KeyCode.Z))
            {
                // we've selected an enemy, raise the event
                Debug.Log("Enemy selected at index: " + selectionIndex);
                EnemySelected?.Invoke(selectionIndex, targetingSingle);
                // disable cursor
                cursorSprite.enabled = false;
                selectingEnemy = false;

                // reset cursor pos
                selectionIndex = 0;
            }

            if (Input.GetKeyUp(KeyCode.X))
            {
                // go back
                BattleManager.Instance.CloseEnemySelection();
            }
        }
    }

    // Called from Battlemanager UpdateBattleState every time state changes
    private void OnBatleStateChange(BattleState newState)
    {
        // battle state change event listener
        switch (newState) 
        {
            case BattleState.SelectingEnemy:
                // enable sprite and movement
                cursorSprite.enabled = true;
                selectingEnemy = true;
                // set position to that of 0th enemy
                transform.position = BattleManager.Instance.enemyUnits[0].transform.position + new Vector3(-1, 0, 0);
                break;
            default:
                cursorSprite.enabled = false;
                selectingEnemy = false;
                break;
        }
    }
}
