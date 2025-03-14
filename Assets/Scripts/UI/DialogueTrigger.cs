using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public GameObject DialoguePanel;
    public GameObject Text;
   // private bool inRange = false;

  

    void Update()
    {           
          if (Input.GetKeyDown(KeyCode.E))
          {
                DialoguePanel.SetActive(true);
                Text.SetActive(true);

          }
    }
}
    //private void OnTriggerEnter2D(Collider2D other)
    //{
        
        
            
        
        
    //}

    //private void OnTriggerExit2D(Collider2D other)
    //{
        
        
           
        
        
    //}




