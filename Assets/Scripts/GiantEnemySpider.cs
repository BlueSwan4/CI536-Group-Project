using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantEnemySpider : BaseEnemy
{
    // class for the boss enemy for the artefact demo

    // get reference to animator
    [SerializeField] GiantEnemySpiderAnimationControl spiderAnim;

    int currentAttack = -1;

    public int GetCurrentAttack() { return currentAttack; }

    // Start is called before the first frame update
    override public void Start()
    {
        // call base method
        base.Start();

        if (spiderAnim == null)
        {
            spiderAnim = GetComponent<GiantEnemySpiderAnimationControl>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override string GetAttackText()
    {
        if (currentAttack != -1)
        {
            return AttackFlavourText[currentAttack];
        }
        else
        {
            return "${unitName} lunges at you!";
        }
    }

    public void BeginSpiderBattle()
    {
        BattleManager.Instance.ReceiveBossData(gameObject, false);
        GameManager.Instance.UpdateGameState(GameState.Fighting);
    }

    // attacks include poison jab, pounce and web trap

    public void PoisonJab()
    {
        // water  move
        if (target != null)
        {
            if (target.typeWeaknesses.Contains(SpellDataSO.elementType.water))
                target.ReceieveDamage(attack * 2);
            else
            {
                target.ReceieveDamage(attack);
            }
        }
    }

    public void Pounce()
    {
        if (target != null)
        {
            target.ReceieveDamage(attack);
        }
    }

    public void WebNet()
    {
        // water  move
        if (target != null)
        {
            target.ReceieveDamage(attack);
        }
    }

    public override IEnumerator UseBasicAttack()
    {
        // play attack anim
        currentAttack = Random.Range(0, 3);

        spiderAnim.ShowAttack();

        yield return new WaitForSeconds(1);

        switch(currentAttack)
        {
            case 0:
                WebNet();
                break;
            case 1:
                Pounce();
                break;
            case 2:
                PoisonJab();
                break;
        }

        EndUnitTurn();
    }
}
