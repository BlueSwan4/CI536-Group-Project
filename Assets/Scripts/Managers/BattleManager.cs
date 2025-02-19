using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    // May need to add functionality to make accessable anywhere later. For now only accessed by GameManager

    public static BattleManager Instance;
    public BattleState State;
    public List<BaseUnit> battleUnits = new List<BaseUnit>();
    [SerializeField] private int turnIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Included so this function will be called everytime the GameManager switches states
            GameManager.OnGameStateChanged += BattleStarted;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    // Start inactive while no battles going
    private void Start()
    {
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
    }

    // Called from GameManager on state change
    private void BattleStarted(GameState gameState)
    {
        if(gameState == GameState.Fighting)
        {
            UpdateBattleState(BattleState.StartBattle);
        }
    }

    public void UpdateBattleState(BattleState newState)
    {
        State = newState;

        switch (newState)
        {
            case BattleState.StartBattle:
                // functions called before starting battle
                // get reference to player
                break;
            case BattleState.PlayerTurn:
                // functions for player turn
                break;
            case BattleState.EnemyTurn:
                break;
            case BattleState.Victory:
                // Not necessarily right place, but should change game state at some point after fight finished
                // clear battle unit array
                battleUnits.Clear();
                // set active scene back to entry point
                GameManager.Instance.UpdateGameState(GameState.Wandering);
                break;
            case BattleState.Defeat:
                // Not necessarily right place, but should change game state at some point after fight finished
                GameManager.Instance.UpdateGameState(GameState.Wandering);
                break;
            case BattleState.Inactive:
                // Need to set inactive at the end of a battle. Just haven't added functionality yet.
                break;
        }
    }

    public void UpdateTurnOrder()
    {
        battleUnits.Sort(TurnComparison);
    }

    public void OnBattleCycleStart()
    {
        // called every time a full set of turns completes
        // (i.e. all player and enemy units have had their turn)
        // NOTE: if we implement status effects the should be evaluated here
        turnIndex = 0;
    }

    private int TurnComparison(BaseUnit a, BaseUnit b)
    {
        if (a.speed >= b.speed) return -1;
        else return 1;
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
    Inactive
}
