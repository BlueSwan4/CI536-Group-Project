using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttackRaise : MonoBehaviour
{
    // Start is called before the first frame update
    public void RaiseAttackEventEnd()
    {
        // this is called when the basic attack animation is ended to raise the turn end event
        BaseUnit.EndUnitTurn();
    }
}
