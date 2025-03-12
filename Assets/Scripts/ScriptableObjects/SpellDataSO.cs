using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellDataSO", menuName = "ScriptableObjects/SpellDataSO")]
public class SpellDataSO : ScriptableObject
{
    public string spellName;
    public int baseDamage;
    public elementType element;
    public targetType target;
    public int spCost;


    // Basic rock paper scissor weakness. Should change later
    public enum elementType
    {
        fire,
        water,
        grass,
        none
    }

    public enum targetType
    {
        single,
        multiple
    }
}
