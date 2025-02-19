using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : BaseUnit
{
    //variables
    [SerializeField] private UnitDataSO enemyStats;

    public void Start()
    {
        speed = 5;
        attack = 5;
        health = 10;
    }
}
