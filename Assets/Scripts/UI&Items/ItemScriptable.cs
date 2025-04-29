using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemScriptable : ScriptableObject
{
    public string itemName;
    public StatToChange statToChange = new StatToChange();
    public int amountToChangeStat;

    public void UseItem()
    {
        if(statToChange == StatToChange.health)
        {
           
        }
    }

    public enum StatToChange
    {
        health
        
    };
}
