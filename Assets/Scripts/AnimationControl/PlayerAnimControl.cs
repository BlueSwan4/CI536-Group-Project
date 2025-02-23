using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimControl : MonoBehaviour
{
    public Animator controller;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPlayerAttack()
    {
        controller.SetTrigger("attacking");
    }
}
