using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BaseUnit : MonoBehaviour
{
    // unit events
    public static event Action UnitTurnEndEvent;
    public static event Action<BaseUnit> UnitDeathEvent; // raised when the unit dies

    // variables (Shared between both player and enemy)
    public int maxHealth;
    public int health;
    public int attack;
    public int speed;

    public float attackDuration = 2.5f;

    public BaseUnit target;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public virtual void UseTurn()
    {

    }

    //Connects to BattleManager
    public virtual void ReceieveDamage(int amount)
    {
        // do damage reduction here
        // overrides may add extra stuff
        health -= amount;

        // check if health <= 0
        if (health <= 0)
        {
            health = 0;
            // raise death event
            UnitDeathEvent.Invoke(this);
        }
    }


    public virtual IEnumerator UseBasicAttack()
    {
        // begins attack using a delay
        // based off of Unity's Coroutine Documentation (2025). Acc: 23/02/2025
        yield return new WaitForSeconds(attackDuration);
        // deal damage to the target
        if (target != null)
        {
            target.ReceieveDamage(attack);
            // end the turn
            // clear current target
            target = null;
            EndUnitTurn();
        }
    }

    //Connects to BattleManager
    public static void EndUnitTurn()
    {
        // method responsible for ending the unit turn and raising the corresponding event
        UnitTurnEndEvent.Invoke();
    }
}
