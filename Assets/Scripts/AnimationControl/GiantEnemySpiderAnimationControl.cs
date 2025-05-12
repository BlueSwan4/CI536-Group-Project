using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantEnemySpiderAnimationControl : BasicEnemyAnimControl
{
    // Start is called before the first frame update
    void Start()
    {
        // subscribe to dialogue start / end events
        Dialogue.DialogueStartedEvent += BeginTalking;
        Dialogue.DialogueEndedEvent += BeginWalking;

        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponentInChildren<Animator>();
        }
    }

    // event handler funcs
    public void BeginTalking(GameObject src)
    {
        if (src.tag == "GiantEnemySpider")
            enemyAnimator.SetTrigger("BattleStarted");
    }

    public void BeginWalking(GameObject src)
    {
        if (src.tag == "GiantEnemySpider")
            enemyAnimator.SetTrigger("BattleEntryWalk");
    }


    public new void ShowAttack()
    {
        enemyAnimator.SetTrigger("UsingTurn");
    }
}
