using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellDataSO", menuName = "ScriptableObjects/SpellDataSO")]
public class SpellDataSO : ScriptableObject
{
    public int baseDamage;
    public elementType element;
    public targetType target;


    // Basic rock paper scissor weakness. Should change later
    public enum elementType
    {
        fire,
        water,
        grass
    }

    public enum targetType
    {
        single,
        multiple
    }
}
