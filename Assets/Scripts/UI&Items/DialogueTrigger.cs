using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public GameObject DialoguePanel;
    public GameObject Text;
    

    [SerializeField] float _triggerDistance = 1f; 
    
    public GameObject Player;

    float _distance;
    public float distance
    {
        get { return _distance; }
        set
        {
            if (value != _distance) 
            {
                _distance = value; 
                playerIsClose = _distance <= _triggerDistance; 
            }
        }
    }

    bool _playerIsClose;
    public bool playerIsClose
    {
        get { return _playerIsClose; }
        set
        {
            if (value == _playerIsClose)
                return; 
            _playerIsClose = value;
           
        }
    }

    void Update()
    {
        // updating the distance from player, eventually triggering the animation
        distance = Vector3.Distance(transform.position, Player.transform.position);
        // catch input if playerIsClose is true
        if (playerIsClose && Input.GetKeyDown(KeyCode.E))
        {
            DialoguePanel.SetActive(true);
            Text.SetActive(true);
        }

    }



    //private bool inRange = false;



    //void Update()
    //{
    //    if (!inRange)       
    //    {
    //        if (Input.GetKeyDown(KeyCode.E))
    //        {
    //            DialoguePanel.SetActive(true);
    //            Text.SetActive(true);

    //        }
    //    }
    //}
    //private void OnTriggerEnter2D(Collider2D collision)
    //{

    //    inRange = true;



    //}
    //private void OnTriggerExit2D(Collider2D other)
    //{

    //    inRange = false;




    //}

}






