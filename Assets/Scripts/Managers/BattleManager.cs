using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;


// Also based off of Tarodev's (2021) GameManager tutorial. Acc: 16/2/2025
public class BattleManager : MonoBehaviour
{
    // May need to add functionality to make accessable anywhere later. For now only accessed by GameManager

    public static BattleManager Instance;
    public BattleState State;
    public List<BaseUnit> battleUnits = new List<BaseUnit>(); // use this for evaluating turn order

    // lists for storing player and enemy uunits seperately - these are for targeting
    public List<Player> playerUnits = new List<Player>();
    public List<BaseEnemy> enemyUnits = new List<BaseEnemy>(); // NOTE: we may need to change the BaseUnit class to be abstract

    public List<BaseEnemy> deadEnemyUnits = new(); // use this to store references to dead enemies

    [SerializeField] public int turnIndex { get; private set; } = 0; // expose this to the inspector for debugging

    // temporary prefab reference for the basic enemy
    // TODO: remove this once we've implemented more enemy types
    [SerializeField] private GameObject baseEnemyPF;

    // Battle Events
    public static event Action<BattleState> BattleStateChange;
    public static event Action BattleEndEvent;

    [Header("Unit Positioning")]
    // single enemy (may want to use this for bosses for example
    public Transform centredPosition;

    public Transform[] standardPositions = new Transform[4]; // positions for standard enemies

    public Transform playerPosition;

    public int activeSpell { get; private set; } = -1; // Rob (2015), acc: 10/3/2025

    [Header("GUI References")]
    public Button fightButton;
    public Button runButton;
    public Button inventoryButton;
    
    public TextMeshProUGUI battleCaptionText;

    //list for HPBars so we can update them efficiently

    public List<EnemyHPBar> enemyHPBars = new List<EnemyHPBar>();


    //references to the specific buttons

    public Button confirmSelection;
    public Button rejectSelection;

    //reference the animtaion controller script and object
    [SerializeField] TransitionsAnimController transitionController;

    //bandage fix so the exit transition works

    public Button spellButton;
    public GameObject spellsPanel;

    public bool canFlee = true;

    [Header("Boss Information (if applicable)")]
    [SerializeField] GameObject bossGameObject;
    [SerializeField] bool isBossPrefab = false;
    [SerializeField] bool isBossFightHappening = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Included so this function will be called everytime the GameManager switches states
            GameManager.OnGameStateChanged += BattleStarted;

            // subscribe to turn end event
            BaseUnit.UnitTurnEndEvent += OnTurnEnd;
            BaseUnit.UnitDeathEvent += OnUnitDeath;
            BattleCursor.EnemySelected += PlayerFight;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    // Start inactive while no battles going
    private void Start()
    {

        
        // add listener for selection completion
        //BattleCursor.EnemySelected += 
        UpdateBattleState(BattleState.Inactive);


    }

    public void Update()
    {
        // Could include lots of if statements to check which BattleState is in to call various functions
    }

    // Can ignore function, just good practice
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= BattleStarted;
        BaseUnit.UnitDeathEvent -= OnUnitDeath;
        BaseUnit.UnitTurnEndEvent -= OnTurnEnd;
    }

    public BossInfo GetBossInfo()
    {
        return new BossInfo(bossGameObject, isBossPrefab);
    }


    // Called from GameManager on state change
    private void BattleStarted(GameState gameState)
    {
        if (gameState == GameState.Fighting)
        {


            // the battle scene will be active by this point so we can attach the methods to the gui
            // grab the gui and attach the necessary functions
            fightButton = GameObject.FindWithTag("FightButton").GetComponent<Button>();
            fightButton.onClick.AddListener(EnableSelectEnemy);

            // attach the run button
            runButton = GameObject.FindWithTag("RunButton").GetComponent<Button>();
            runButton.onClick.AddListener(OpenFleeSelection);
            runButton.interactable = canFlee;

            // attach inventory button
            inventoryButton = GameObject.FindWithTag("InventoryButton").GetComponent<Button>();
            inventoryButton.onClick.AddListener(InventoryManager.Instance.OpenInventory);

            spellButton = GameObject.FindWithTag("SpellButton").GetComponent<Button>();
            spellButton.onClick.AddListener(EnableSpellSelection);

            spellsPanel = GameObject.FindWithTag("SpellsPanel");

            // go through the spell buttons and attach the usespell method
            for (int i = 0; i < spellsPanel.transform.childCount; i++)
            {
                spellsPanel.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(GetSpellCaster(i)); // use a helper function to prevent late binding (Martelli et al. 2010)
            }

            spellsPanel.SetActive(false); // disable on start

            UpdateBattleState(BattleState.StartBattle);

            //find the conformation buttons and deactivate them
            
            confirmSelection = GameObject.FindWithTag("ConfirmSelection").GetComponent<Button>();
            rejectSelection = GameObject.FindWithTag("RejectSelection").GetComponent<Button>();


            rejectSelection.gameObject.SetActive(false);

            confirmSelection.gameObject.SetActive(false);

            //find the text
            battleCaptionText = GameObject.FindWithTag("Description").GetComponent<TextMeshProUGUI>();

            //transitions
            //im finding the animation controller here
            transitionController = GameObject.FindWithTag("Transitions").GetComponent<TransitionsAnimController>();
        }
        else
        {
            // set gui references to null
            fightButton = null;
            runButton = null;
            spellButton = null;
            battleCaptionText = null;

            UpdateBattleState(BattleState.Inactive);
        }
    }

    public void UpdateBattleState(BattleState newState)
    {
        State = newState;

        switch (newState)
        {
            case BattleState.StartBattle:
                // functions called before starting battle
                // spawn enemies
                if (isBossFightHappening)
                {
                    RollEnemiesScripted(bossGameObject, isBossPrefab);
                }
                else
                {
                    RollEnemies();
                }
                
                // set turn order based on enemy spawns
                // add player to battle units and players list
                playerUnits.Add(GameManager.Instance.playergameObj.GetComponent<Player>());
                battleUnits.Add(GameManager.Instance.playergameObj.GetComponent<Player>());
                UpdateTurnOrder();
                // set player position
                Debug.Log("setting player pos");
                GameManager.Instance.playergameObj.transform.position = playerPosition.position;
                // reset turn index to 0
                turnIndex = 0;

                break;
            case BattleState.PlayerTurn:
                // functions for player turn
                // enable battle control gui elements
                runButton.interactable = canFlee;
                fightButton.interactable = true;
                spellButton.interactable = true;
                break;
            case BattleState.EnemyTurn:
                runButton.interactable = false;
                fightButton.interactable = false;
                spellButton.interactable = false;
                HandleEnemyTurn(); // call this to go through the current enemy's turn
                break;
            case BattleState.Victory:
                // Not necessarily right place, but should change game state at some point after fight finished
                // clear battle unit array
                ClearBattleUnits();
                // reset spell panel active state
                SetUpForNextEncounter();
                // raise battle won event (GameManager);
                BattleEndEvent.Invoke();
                break;
            case BattleState.Defeat:
                // Not necessarily right place, but should change game state at some point after fight finished
                // clear unit lists
                ClearBattleUnits();
                SetUpForNextEncounter();
                Debug.Log("game over");
                BattleEndEvent.Invoke(); // GameManager
                break;
            case BattleState.SelectingEnemy:
                break;
            case BattleState.Inactive:
                // Need to set inactive at the end of a battle. Just haven't added functionality yet.
                break;
        }

        // raise battle state change event
        BattleStateChange?.Invoke(State);

        // if we are in the start state, update battle state again to be either player or enemy turn
        if (State == BattleState.StartBattle)
        {
            if (battleUnits[0] is Player)
            {
                UpdateBattleState(BattleState.PlayerTurn);
            }
            else
            {
                UpdateBattleState(BattleState.EnemyTurn);
            }
        }
    }

    private void SetUpForNextEncounter()
    {
        spellsPanel.SetActive(true);
        battleCaptionText.SetText(" ");
        rejectSelection.gameObject.SetActive(true);
        confirmSelection.gameObject.SetActive(true);

        for(int i = 0; i < enemyHPBars.Count; i ++)
        {
            enemyHPBars[i].gameObject.SetActive(true); 
            enemyHPBars[i].ResetHPBar();
        }

        // reset boss data
        bossGameObject = null;
        isBossPrefab = false;
        isBossFightHappening = false;
        canFlee = true;
    }

    public void UpdateTurnOrder()
    {
        battleUnits.Sort(TurnComparison);
    }

    // Called from BaseUnit EndUnitTurn()
    public void OnTurnEnd()
    {
        // called every time a turn is ends
        turnIndex++;

        Debug.Log("Current turn index: " + turnIndex.ToString());

        // check if we need to start a new cycle
        
        // check if all enemy units are dead
        if (battleUnits.Count > 0)
        {
            // update battle units to account for dead enemies
            for (int enemyIndex = enemyUnits.Count - 1; enemyIndex >= 0; enemyIndex--)
            {
                // go through list backwards to avoid issues
                if (deadEnemyUnits.Contains(enemyUnits[enemyIndex]))
                {
                    var unit = enemyUnits[enemyIndex];
                    // remove the unit
                    deadEnemyUnits.Remove(unit);
                    // remove from battle units and enemy units array
                    battleUnits.Remove(unit);
                    enemyUnits.Remove(unit);

                    Destroy(unit.gameObject); // destroy the game object
                }
            }

            // check if enemies are dead
            if (enemyUnits.Count == 0)
            {
                UpdateBattleState(BattleState.Victory);
            }
            else
            {
                if (turnIndex >= battleUnits.Count)
                {
                    turnIndex = 0;
                }

                // update battle state to determine next turn
                if (battleUnits[turnIndex] is BaseEnemy)
                {
                    UpdateBattleState(BattleState.EnemyTurn);
                }
                else if (battleUnits[turnIndex] is Player)
                {
                    UpdateBattleState(BattleState.PlayerTurn);
                }
            }
        }
    }

    private int TurnComparison(BaseUnit a, BaseUnit b)
    {
        if (a.speed >= b.speed) return -1;
        else return 1;
    }

    // NEEDS CHANGING
    private void RollEnemies()
    {
        // TODO: add randomised enemy spawning once we have more enemy types

        Debug.Log("Spawning Enemies");
        // use this to determine what enemies to spawn
        int enemyCount = UnityEngine.Random.Range(1, 5);

        for (int i = 0; i < enemyCount; i++)
        {
            // spawn an enemy - make the parent the battle scene root
            GameObject enemy = Instantiate(baseEnemyPF, 
                GameObject.FindWithTag("BattleRootRef").GetComponent<RootReferenceHolder>().rootObject.transform);

            // set enemy position to that of the enemy hook
            enemy.transform.position = standardPositions[i].position;
            // add enemy to recquired lists
            battleUnits.Add(enemy.GetComponent<BaseEnemy>());
            enemyUnits.Add(enemy.GetComponent<BaseEnemy>());
        }

        Debug.Log("Rolled for " + enemyUnits.Count + " enemies");

        //activate the hpBars
        for (int i = 0; i < enemyHPBars.Count; i++)
        {
            
            enemyHPBars[i].gameObject.SetActive(true);
            enemyHPBars[i].canUpdateHpBar = true;
            enemyHPBars[i].SetInitialHpProgress();
        }

        
    }

    private void RollEnemiesScripted(GameObject bossEnemy, bool enemyIsPrefab)
    {
        // use this to add specific enemies (i.e. for story battles / bosses)
        GameObject spawnedBoss = null;

        if (enemyIsPrefab)
        {
            spawnedBoss = Instantiate(bossEnemy, centredPosition.position, Quaternion.identity); // use this if spawning from prefab
        }
        else
        {
            spawnedBoss = bossEnemy;
            bossEnemy.transform.position = centredPosition.position;
            bossEnemy.transform.rotation = Quaternion.identity;
        }

        // add boss enemy to enemy units
        enemyUnits.Add(spawnedBoss.GetComponent<BaseEnemy>());
        battleUnits.Add(spawnedBoss.GetComponent<BaseEnemy>());

    }

    public void ReceiveBossData(GameObject boss, bool isPrefab)
    {
        bossGameObject = boss;
        isBossPrefab = isPrefab;
        isBossFightHappening = true;
        canFlee = false;
    }

    public void EnableSpellSelection()
    {
        CloseFleeSelection();

        // check we are on player turn
        if (battleUnits[turnIndex] is not Player)
            return;

        Player currentPlayer = battleUnits[turnIndex] as Player;

        Debug.Log("Enabling spells");
        // enable the spells panel and update the text on the buttons to match the spell names
        spellsPanel.SetActive(true);
        // update caption text
        battleCaptionText.SetText("Select a Spell");

        Debug.Log("Current spells: " + currentPlayer.playerSpells.Count.ToString());

        for (int i = 0; i < currentPlayer.playerSpells.Count; i++)
        {
            Button newBtn = spellsPanel.transform.GetChild(i).GetComponent<Button>();
            newBtn.GetComponentInChildren<TextMeshProUGUI>().SetText(currentPlayer.playerSpells[i].spellName);
            newBtn.gameObject.SetActive(true);
            // check if we have enough sp
            newBtn.interactable = currentPlayer.playerSpells[i].spCost <= currentPlayer.sp;
        }
    }

    public void UseSpell(int spellIndex)
    {
        Debug.Log("Using spell");
        activeSpell = spellIndex;

        Debug.Log("Set active spell as: " + activeSpell.ToString());

        // disable all spell buttons
        for (int i = 0; i < spellsPanel.transform.childCount; i++)
        {
            spellsPanel.transform.GetChild(i).gameObject.SetActive(false);
        }

        spellsPanel.SetActive(false);
        
        UpdateBattleState(BattleState.SelectingEnemy);
    }

    public UnityAction GetSpellCaster(int spellIndex)
    {
        // helper function to prevent late binding, based off of the StackOverflow answer by Alex Martelli et al. (2010), acc: 11/3/2025
        return () => UseSpell(spellIndex); // we return a lambda function to allow data to be passed for the event (Senshi, 2014)
    }

    // Connected to BattleCursor Update()
    public void PlayerFight(int target, bool hitAll)
    {
        // this is called once the enemy is selected
        // is subscribed to the EnemySelected event
        // check if we are on player's turn
        Debug.Log("Receieved enemy selection");

        if (turnIndex >= 0 && turnIndex < battleUnits.Count)
        {
            if (battleUnits[turnIndex] is not Player)
            {
                Debug.Log("Not on player turn");
                return;
            }

            // disable fight / run buttons / spell buttons (no turning back now) prevents plaer being able to click after choosing one
            runButton.interactable = false;
            fightButton.interactable = false;
            spellButton.interactable = false;

            Debug.Log("On player turn, carrying out movement");
            // we are on player turn, attack the enemy
            var player = battleUnits[turnIndex] as Player;

            player.OnEnemySelected(target);
            // remove active spell if we have one
            activeSpell = -1;
        }
        else
        {
            return;
        }
    }

    public void HandleEnemyTurn()
    {
        if (turnIndex >= 0 && turnIndex < battleUnits.Count)
        {
            if (battleUnits[turnIndex] is not BaseEnemy)
                return;

            // if we are dead skip to next turn
            if (deadEnemyUnits.Contains(battleUnits[turnIndex] as BaseEnemy))
            {
                BaseUnit.EndUnitTurn();
            }
            else
            {
                var enemy = battleUnits[turnIndex] as BaseEnemy;
                enemy.UseTurn();
            }
        }
        else
        {
            return;
        }
    }



    public void OpenFleeSelection()
    {
        UpdateBattleState(BattleState.PlayerTurn); // in case we are in the enemy selection state
        // also disable the spell buttons and panel

        for (int i = 0; i < spellsPanel.transform.childCount; i++)
        {
            spellsPanel.transform.GetChild(i).gameObject.SetActive(false);
        }

        spellsPanel.SetActive(false);

        battleCaptionText.text = " are you sure you want to flee? ";

        // re enable the selection buttons
        rejectSelection.gameObject.SetActive(true);

        confirmSelection.gameObject.SetActive(true);

        //add listeners
        rejectSelection.onClick.AddListener(CloseFleeSelection); // moved this here as the listeners only need to be added when the battle starts
        confirmSelection.onClick.AddListener(FleeBattle);
    }

    public void CloseFleeSelection()
    {
        battleCaptionText.text = " ";

        rejectSelection.gameObject.SetActive(false);

        confirmSelection.gameObject.SetActive(false);
    }

    public void CloseSpellSelection()
    {
        battleCaptionText.SetText(" ");
        spellsPanel.SetActive(false);
    }

    public void CloseEnemySelection()
    {
        // call closespellselection in case we are selecting a spell target
        CloseSpellSelection();
        UpdateBattleState(BattleState.PlayerTurn);
    }

    public void FleeBattle()
    {
        //fade out
        transitionController.CrossFadeOut();
        battleCaptionText.text = " ";

        SetUpForNextEncounter(); 
        //THIS IS A BANDAGE FIX, i WILL ASK ABOUT THIS LATER
        StartCoroutine(ExitBattle());
    }

    IEnumerator ExitBattle()
    {
        yield return new WaitForSeconds(1);

        // currently auto leave battle, with no reward
        ClearBattleUnits();
        spellsPanel.SetActive(true);
        UpdateBattleState(BattleState.Inactive);
        GameManager.Instance.UpdateGameState(GameState.Wandering);

        // TODO: transition back to overworld
    }

    public void EnableSelectEnemy()
    {
        
        // hide spell panel if necessary
        spellsPanel.SetActive(false);
        UpdateBattleState(BattleState.SelectingEnemy);
        CloseFleeSelection();
    }

    // Called every time a unit dies from BaseUnit
    public void OnUnitDeath(BaseUnit deadUnit)
    {
        // check unit type
        if (deadUnit is BaseEnemy)
        {
            // enemy died, add to dead units list
            deadEnemyUnits.Add(deadUnit as BaseEnemy);
            
        }
        else if (deadUnit is Player)
        {
            // rip player 2025 to 2025
            // we don't currently have a game over so just go back to the wander state
            UpdateBattleState(BattleState.Defeat);
        }
    }

    private void ClearBattleUnits()
    {
        battleUnits.Clear();

        for (int i = 0; i < enemyUnits.Count; i++)
        {
            Destroy(enemyUnits[i].gameObject);
        }

        enemyUnits.Clear();
    }

    //after the player attacks we can update the hp bars
    public void UpdateHPBars()
    {
        foreach(EnemyHPBar hpBar in enemyHPBars)
        {
            hpBar.UpdateHp(); 
        }
    }
    
}

public struct BossInfo
{
    public GameObject BossGameObject;
    public bool isBossAPrefabObject;

    public BossInfo(GameObject boss, bool isPf)
    {
        BossGameObject = boss;
        isBossAPrefabObject = isPf;
    }
}

// Add any states needed for the battle here
public enum BattleState
{
    StartBattle,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Defeat,
    Inactive,
    SelectingEnemy
}
