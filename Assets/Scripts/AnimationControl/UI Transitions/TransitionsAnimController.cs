using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionsAnimController : MonoBehaviour
{
    public Animator controller; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CrossFadeIn()
    {
        controller = GameObject.FindWithTag("Transitions_go").GetComponent<Animator>();
        controller.SetTrigger("FadeIn");
    }

    public void CrossFadeOut()
    {
        controller = GameObject.FindWithTag("Transitions_go").GetComponent<Animator>();
        controller.SetTrigger("FadeOut");
    }
}
