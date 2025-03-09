using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellDataSO", menuName = "ScriptableObjects/SpellDataSO")]
public class SpellDataSO : ScriptableObject
{
    // Basic rock paper scissor weakness. Should change later
    public enum spellType
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

    public int baseDamage;
}
