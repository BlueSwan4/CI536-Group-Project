using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseEnemy : BaseUnit
{
    //variables
    [SerializeField] private UnitDataSO enemyStats;
    [SerializeField] private BasicEnemyAnimControl anim;

    [SerializeField] protected string[] IntroductionFlavourText;
    [SerializeField] protected string[] AttackFlavourText;

    public virtual void Start()
    {
        // on instantiation, read in data from associated scriptable object
        maxHealth = enemyStats.maxUnitHealth;
        health = enemyStats.maxUnitHealth;
        attack = enemyStats.baseAttack;
        speed = enemyStats.baseSpeed;
        unitName = enemyStats.unitName;
    }

    public override void UseTurn()
    {
        // basic attack in the case of the basic enemy
        // target player
        target = GameManager.Instance.playergameObj.GetComponent<Player>();
        anim.ShowAttack();
        StartCoroutine("UseBasicAttack");
    }

    public virtual string GetIntroText()
    {
        return GetRandomFormattedString(IntroductionFlavourText);
    }

    public virtual string GetAttackText()
    {
        return GetRandomFormattedString(AttackFlavourText);
    }

    protected string GetRandomFormattedString(string[] arr)
    {
        // based on documentation by Kulikov et al. (2024)
        string msg = $""; // append to this string to allow for interpolation
        msg = msg + arr[Random.Range(0, arr.Length - 1)];

        return msg;
    }
}
