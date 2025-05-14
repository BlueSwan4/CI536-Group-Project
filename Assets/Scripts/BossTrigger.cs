using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private GameObject spiderBossObject;
    [SerializeField] private Transform playerDestination;

    // Start is called before the first frame update
    void Start()
    {
        // subscribe to player movement end event
        PlayerMovement.MovementCompleted += OnMovingToPositionEnded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMovingToPositionEnded(GameObject src)
    {
        if (src == gameObject)
        {
            // we are the one who requested the move
            Debug.Log("starting spider dialogue");
            spiderBossObject.GetComponent<DialogueTrigger>().EnableDialogue(); // start cutscene dialogue, spider will handle the rest
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("starting the boss intro");
            collision.GetComponent<PlayerMovement>().MoveToLocation(playerDestination.position, 5, gameObject);
        }
    }
}
