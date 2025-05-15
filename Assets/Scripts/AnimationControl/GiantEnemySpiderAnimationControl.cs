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

        BattleManager.BattleStateChange += GoToBattleIdle;

        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponentInChildren<Animator>();
        }
    }

    // event handler funcs
    public void BeginTalking(GameObject src)
    {
        if (src.tag == "GiantEnemySpider")
            enemyAnimator.SetTrigger("DialogueStarted");
    }

    public void BeginWalking(GameObject src)
    {
        Debug.Log("dialogue ended");
        if (src.tag == "GiantEnemySpider")
        {
            enemyAnimator.SetTrigger("BattleEntryWalk");
            StartCoroutine("DelayForBattle");
        }
    }

    public void GoToBattleIdle(BattleState newState)
    {
        if (newState != BattleState.Inactive && newState != BattleState.EnemyTurn)
        {
            Debug.Log("detected battle start on spider");
            enemyAnimator.SetTrigger("BattleStarted");
        }
    }

    public new void ShowAttack()
    {
        enemyAnimator.SetTrigger("UsingTurn");
    }

    public IEnumerator DelayForBattle()
    {
        yield return new WaitForSeconds(1);
        GetComponent<GiantEnemySpider>().BeginSpiderBattle();
    }
}
