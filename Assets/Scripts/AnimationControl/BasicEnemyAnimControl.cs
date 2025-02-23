using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyAnimControl : MonoBehaviour
{
    public Animator enemyAnimator;

    public void ShowAttack()
    {
        enemyAnimator.SetTrigger("attacking");
    }
}
