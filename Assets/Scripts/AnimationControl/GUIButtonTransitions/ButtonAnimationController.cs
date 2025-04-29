using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimationController : MonoBehaviour
{
    public Animator animator;

    public GameObject fire; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ButtonEntered()
    {
        animator.SetBool("ButtonEntered", true); 
        fire.SetActive(true);
    }

    public void ButtonExit()
    {
        animator.SetBool("ButtonEntered", false);
        fire.SetActive(false);
    }

    public void ButtonClicked()
    {
        animator.SetTrigger("ButtonClicked");

        fire.SetActive(false);
    }


}
