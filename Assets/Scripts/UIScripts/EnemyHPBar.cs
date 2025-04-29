using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    //variables
    [SerializeField] private BaseEnemy unitToHaveHpBar;
    [SerializeField] private Slider slider;
    public BattleManager battleManager;
    //set this to the unit number you want to target
    public int unitNumber;

    float timerTimer = 0.5f;
    float timerCounter;

    public bool canUpdateHpBar = false; 
    

    // Start is called before the first frame update
    void Start()
    {
        //initialise 
        battleManager = GameObject.FindWithTag("BattleManager").GetComponent<BattleManager>();
        slider = this.GetComponent<Slider>();
        //add this gameObject to the enemyHPBar list in the battle manager
        battleManager.enemyHPBars.Add(this.GetComponent<EnemyHPBar>()); 
        //set the hpBarProgress
        SetInitialHpProgress();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canUpdateHpBar)
        {

            timerCounter += Time.deltaTime; 
            if(timerCounter > timerTimer)
            {
                UpdateHp();
            
            }
        }

    }

    public void SetInitialHpProgress()
    {
        Debug.Log("setting initial enemy hp bars"); 

        if (battleManager.enemyUnits.Count > unitNumber)
        {
            for (int i = 0; i < battleManager.enemyUnits.Count; i++)
            {
                if (i == unitNumber)
                {
                    
                    unitToHaveHpBar = battleManager.enemyUnits[i].GetComponent<BaseEnemy>();
                    slider.maxValue = battleManager.enemyUnits[i].maxHealth;
                    slider.value = battleManager.enemyUnits[i].health;

                    canUpdateHpBar = true; 
                    

                }
                

            }
        }
        else
        {
            this.gameObject.SetActive(false);
            
        }

    }


    public void UpdateHp()
    {

        if (unitToHaveHpBar.health > 0.1)
        {
            slider.maxValue = unitToHaveHpBar.maxHealth;
            slider.value = unitToHaveHpBar.health;

        }

        else 
        {

            this.gameObject.SetActive(false); 
        }
        
    }

    public void ResetHPBar()
    {
        canUpdateHpBar = false;
        unitToHaveHpBar = null;

        slider.value = 0;

        this.gameObject.SetActive(false);

    }
}
