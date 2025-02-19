using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Allows GameManager to be called from anywhere
    public static GameManager Instance;

    public GameState State;

    public static event Action<GameState> OnGameStateChanged;

    public GameObject playergameObj;

    [Header("Random Encounter Data")]
    public int stepsTakenInOverworld = 0;
    public bool inSafeArea = false; // flag to limit encounters to "unsafe" areas


    // TODO: change to don't destoy on load when we have extra game areas outside of the original and battle area
    void Awake()
    {
        if (Instance == null)
        {
            // set up game manager static instance
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // scene loading has to be done from start, not awake
        Scene startScene = SceneManager.GetActiveScene();
        SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(startScene);
        UpdateGameState(GameState.Wandering);
        // note the steps taken - we want to run a random encounter check at a regular interval
        stepsTakenInOverworld = 0;
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch(newState)
        {
            case GameState.Wandering:
                stepsTakenInOverworld = 0;
                break;
            case GameState.Fighting: 
                // Activate the BattleManager
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private void FixedUpdate()
    {
        switch (State)
        {
            case GameState.Fighting:
                break;
            case GameState.Wandering:
                bool encounter = CheckForRandomEncounter();
                if (encounter)
                {
                    // switch scene to battle scene
                    // also move necessary objects to batle scene
                    SceneManager.MoveGameObjectToScene(playergameObj, SceneManager.GetSceneByName("BattleScene"));
                    SceneManager.MoveGameObjectToScene(transform.parent.gameObject, SceneManager.GetSceneByName("BattleScene"));

                    // set battle scene as the active scene, then update game state
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName("BattleScene"));

                    // update game state
                    UpdateGameState(GameState.Fighting);
                    // put a console message to signify battle entry
                    Debug.Log("Entering battle");
                }
                break;
        }
        
    }

    private bool CheckForRandomEncounter()
    {
        if (inSafeArea)
            return false;
        // if we haven't moved then don't trigger an encounter
        if (stepsTakenInOverworld == 0)
            return false;

        // use steps count to determine if we have encountered an enemy

        float res = UnityEngine.Random.Range(0f, 50f) - stepsTakenInOverworld;

        return res < 20; // TODO: replace this with the final random encounter formula
    }
}

// Add more states as necessary, these are for actions that fundamentally change what happens in the game
public enum GameState
{
    Wandering,
    Fighting
}
