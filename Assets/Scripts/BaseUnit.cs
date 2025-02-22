using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    // unit events
    public static event Action unitEndEvent;

    // variables (Shared between both player and enemy)
    public int health;
    public int attack;
    public int speed;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public static void EndUnitTurn()
    {
        // method responsible for ending the unit turn and raising the corresponding event
        unitEndEvent.Invoke();
    }
}
