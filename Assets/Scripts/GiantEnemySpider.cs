using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantEnemySpider : BaseEnemy
{
    // class for the boss enemy for the artefact demo

    // Start is called before the first frame update
    override public void Start()
    {
        // call base method
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginSpiderBattle()
    {
        BattleManager.Instance.ReceiveBossData(gameObject, false);
        GameManager.Instance.UpdateGameState(GameState.Fighting);
    }

    // attacks include poison jab, pounce and web trap

    public void BasicAttack()
    {

    }

    public void PoisonJab()
    {
        // water  move
    }

    public void Pounce()
    {

    }
}
