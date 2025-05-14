using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Overworld_PlayerHPBar : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Slider slider;
    public TextMeshProUGUI sliderToolTip; 

    //this boolien can be used to tell if its the hp bar ot mana bar
    public bool hpBar = true;
    // Start is called before the first frame update
    void Start()
    {
        //initialise
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hpBar)
        {
            //20 is being set here because the max health value is 0 (the slider will display 0)
            slider.maxValue = 20;
            slider.value = player.health;
            sliderToolTip.text = player.health.ToString() + " / " + slider.maxValue;
        }
        else
        {
            //im setting the max value to 50 here as the player does not have a max value variable for sp
            slider.maxValue = 50;
            slider.value = player.sp;
            sliderToolTip.text = player.sp.ToString() + " / " + slider.maxValue;
        }
    }
}
