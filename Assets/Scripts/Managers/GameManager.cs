using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Allows GameManager to be called from anywhere
    public static GameManager Instance;

    public GameState State = GameState.GameStart;

    public static event Action<GameState> OnGameStateChanged;

    public GameObject playergameObj;

    private Scene overworldScene;

    [Header("Random Encounter Data")]
    public float stepsTakenInOverworld = 0;
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
        switch(newState)
        {
            case GameState.Wandering:
                if (State == GameState.Fighting) // checking old state
                    TransitionToOverworldFromBattle();

                stepsTakenInOverworld = 0;
                break;
            case GameState.Fighting:
                // Activate the BattleManager
                // move to battle scene
                TransitionToBattleFromOverworld();
                break;
        }

        State = newState; // this has been moved down here to allow for checking of the old state
                          // i.e. interaction that's specific to certain state transitions
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
                    Debug.Log("Encounter");
                    UpdateGameState(GameState.Fighting);
                }
                break;
        }
        
    }

    private bool CheckForRandomEncounter()
    {
        if (inSafeArea)
            return false;
        // if we haven't moved then don't trigger an encounter
        if (stepsTakenInOverworld < 5)
            return false;

        // use steps count to determine if we have encountered an enemy

        float res = UnityEngine.Random.Range(0f, 50f) - stepsTakenInOverworld;

        return res < 20; // TODO: replace this with the final random encounter formula
    }

    private void TransitionToBattleFromOverworld()
    {
        // get a "hook" for the scene to return to on battle conclusion
        overworldScene = SceneManager.GetActiveScene();
        // move necessary objects to battle scene
        SceneManager.MoveGameObjectToScene(playergameObj, SceneManager.GetSceneByName("BattleScene"));
        SceneManager.MoveGameObjectToScene(transform.parent.gameObject, SceneManager.GetSceneByName("BattleScene"));
        // set active scene to battle
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("BattleScene"));
        // get battle root
        var root = GameObject.FindWithTag("BattleRootRef").GetComponent<RootReferenceHolder>(); // find root
        root.rootObject.SetActive(true);
        // disable overworld root
        GameObject.FindWithTag("OverworldRootRef").GetComponent<RootReferenceHolder>().rootObject.SetActive(false);
    }

    private void TransitionToOverworldFromBattle()
    {
        // move player and managers back to overworld scene
        SceneManager.MoveGameObjectToScene(playergameObj, overworldScene);
        SceneManager.MoveGameObjectToScene(transform.parent.gameObject, overworldScene);

        // disable battle scene root
        GameObject.FindWithTag("BattleRootRef").GetComponent<RootReferenceHolder>().rootObject.SetActive(false);
        // set overworld as active scene
        SceneManager.SetActiveScene(overworldScene);
        // enable overworld root
        GameObject.FindWithTag("OverworldRootRef").GetComponent<RootReferenceHolder>().rootObject.SetActive(true);
    }
}

// Add more states as necessary, these are for actions that fundamentally change what happens in the game
public enum GameState
{
    GameStart,
    Wandering,
    Fighting
}
