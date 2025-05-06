using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptable", menuName ="ScriptableObjects/ItemScriptable")]
public class ItemScriptable : ScriptableObject
{
    public string itemName;
    public StatToChange statToChange = new StatToChange();
    public int amountToChangeStat;

    public void UseItem()
    {
        Player playerObj = GameManager.Instance.playergameObj.GetComponent<Player>();

        switch(statToChange)
        {
            case StatToChange.health:
                if (playerObj.health + amountToChangeStat >= playerObj.maxHealth)
                {
                    playerObj.health = playerObj.maxHealth;
                }
                else
                {
                    playerObj.health += amountToChangeStat;
                }
                break;
            case StatToChange.mana:
                if (playerObj.sp + amountToChangeStat >= playerObj.maxSP)
                {
                    playerObj.sp = playerObj.maxSP;
                }
                else
                {
                    playerObj.sp += amountToChangeStat;
                }
                break;
        }
    }

    public enum StatToChange
    {
        health,
        mana
    };
}
