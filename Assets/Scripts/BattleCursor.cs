using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCursor : MonoBehaviour
{
    [Header("GameObject References")]
    public SpriteRenderer cursorSprite;

    // event for enemy selection - raised when enemy is selected
    public static event Action<int> EnemySelected;
    private int selectionIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // subscribe to the battle state change event
        BattleManager.BattleStateChange += OnBatleStateChange;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (BattleManager.Instance.State == BattleState.SelectingEnemy)
        {
            // movement - use wasd for selecting enemies
            // a / left - move cursor left
            // d / right - move cursor right

        }
    }

    private void OnBatleStateChange(BattleState newState)
    {
        // battle state change event listener
        switch (newState) 
        {
            case BattleState.SelectingEnemy:
                // enable sprite and movement
                cursorSprite.enabled = true;
                break;
            default:
                cursorSprite.enabled = false;
                break;
        }
    }
}
