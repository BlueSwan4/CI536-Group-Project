using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentLocation : MonoBehaviour
{

    public TextMeshProUGUI tmp_text;
    public GameObject player; 
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");  
    }

    // Update is called once per frame
    void Update()
    {
        tmp_text.text = "x: " + Mathf.Round(player.transform.position.x).ToString() + " y: " + Mathf.Round(player.transform.position.y).ToString(); 
    }
}
