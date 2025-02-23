using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseEnemy : BaseUnit
{
    //variables
    [SerializeField] private UnitDataSO enemyStats;

    public void Start()
    {
        // on instantiation, read in data from associated scriptable object
        health = enemyStats.maxUnitHealth;
        attack = enemyStats.baseAttack;
        speed = enemyStats.baseSpeed;
    }

    public override void UseTurn()
    {
        // basic attack in the case of the basic enemy
        // target player
        target = GameManager.Instance.playergameObj.GetComponent<Player>();
        StartCoroutine("UseBasicAttack");
    }
}
