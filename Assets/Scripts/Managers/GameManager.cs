using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Allows GameManager to be called from anywhere
    public static GameManager Instance;

    public GameState State;

    public static event Action<GameState> OnGameStateChanged;

    // Need to change Awake() so only creates new instance if one
    // does not already exist. Also need to change to don't destoy on load
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.Wandering);
        // note the time - we want to run a random encounter check at a regular interval
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch(newState)
        {
            case GameState.Wandering:
                break;
            case GameState.Fighting: 
                // Activate the BattleManager
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }
}

// Add more states as necessary, these are for actions that fundamentally change what happens in the game
public enum GameState
{
    Wandering,
    Fighting
}
