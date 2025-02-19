using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Use this as a storage for enemy data (e.g. max health, speed etc.)
// Put values in here that don't change (don't store health in here as changes)

[CreateAssetMenu(fileName = "UnitDataSO", menuName = "ScriptableObjects/UnitDataSO")]
public class UnitDataSO : ScriptableObject
{
    // Add more as necessary
    public string unitName;
    public int maxUnitHealth;
    public int baseAttack;
    public int baseSpeed;
}
