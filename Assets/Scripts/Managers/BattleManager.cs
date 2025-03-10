using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


// Also based off of Tarodev's (2021) GameManager tutorial. Acc: 16/2/2025
public class BattleManager : MonoBehaviour
{
    // May need to add functionality to make accessable anywhere later. For now only accessed by GameManager

    public static BattleManager Instance;
    public BattleState State;
    public List<BaseUnit> battleUnits = new List<BaseUnit>(); // use this for evaluating turn order

    // lists for storing player and enemy uunits seperately - these are for targeting
    public List<Player> playerUnits = new List<Player>();
    public List<BaseEnemy> enemyUnits = new List<BaseEnemy>(); // NOTE: we may need to change the BaseUnit class to be abstract and 

    [SerializeField] private int turnIndex = 0; // expose this to the inspector for debugging

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

    [Header("GUI References")]
    public Button fightButton;
    public Button runButton;
    public Button spellButton;
    public Text battleCaptionText;

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
            runButton.onClick.AddListener(FleeBattle);

            spellButton = GameObject.FindWithTag("SpellButton").GetComponent<Button>();

            UpdateBattleState(BattleState.StartBattle);
        }
        else
        {
            // set gui references to null
            fightButton = null;
            runButton = null;
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
                RollEnemies();
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
                runButton.interactable = true;
                fightButton.interactable = true;
                break;
            case BattleState.EnemyTurn:
                runButton.interactable = false;
                fightButton.interactable = false;
                HandleEnemyTurn(); // call this to go through the current enemy's turn
                break;
            case BattleState.Victory:
                // Not necessarily right place, but should change game state at some point after fight finished
                // clear battle unit array
                ClearBattleUnits();

                // raise battle won event (GameManager);
                BattleEndEvent.Invoke();
                break;
            case BattleState.Defeat:
                // Not necessarily right place, but should change game state at some point after fight finished
                // clear unit lists
                ClearBattleUnits();
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
    }

    private void RollEnemiesScripted()
    {
        // use this to add specific enemies (i.e. for story battles / bosses)
    }

    // Connected to BattleCursor Update()
    public void PlayerFight(int target)
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

            // disable fight / run buttons (no turning back now)
            runButton.interactable = false;
            fightButton.interactable = false;

            Debug.Log("On player turn, carrying out movement");
            // we are on player turn, attack the enemy
            var player = battleUnits[turnIndex] as Player;

            player.OnEnemySelected(target);
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

            var enemy = battleUnits[turnIndex] as BaseEnemy;

            enemy.UseTurn();
        }
        else
        {
            return;
        }
    }

    public void FleeBattle()
    {
        // currently auto leave battle, with no reward
        ClearBattleUnits();
        GameManager.Instance.UpdateGameState(GameState.Wandering);

        // TODO: transition back to overworld
    }

    public void EnableSelectEnemy()
    {
        UpdateBattleState(BattleState.SelectingEnemy);
    }

    // Called every time a unit dies from BaseUnit
    public void OnUnitDeath(BaseUnit deadUnit)
    {
        // check unit type
        if (deadUnit is BaseEnemy)
        {
            // enemy died
            enemyUnits.Remove(deadUnit as BaseEnemy);
            battleUnits.Remove(deadUnit);

            Destroy(deadUnit.gameObject);

            // check amount of remaining enemies
            if (!(enemyUnits.Count > 0))
            {
                // battle is won
                UpdateBattleState(BattleState.Victory);
            }
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
