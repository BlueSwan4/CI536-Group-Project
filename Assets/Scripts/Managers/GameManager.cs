using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Based off of Tarodev (2021)'s Game Manager tutorial. Acc: 16/2/2025

public class GameManager : MonoBehaviour
{
    // Allows GameManager to be called from anywhere
    public static GameManager Instance;

    public GameState State = GameState.GameStart;

    public static event Action<GameState> OnGameStateChanged;

    public GameObject playergameObj;

    private Scene overworldScene;
    private Vector2 overworldPosition = Vector2.zero;

    [Header("Random Encounter Data")]
    public float stepsTakenInOverworld = 0;

    // Not used at the moment, IMPLEMENT LATER
    public bool inSafeArea = true; // flag to limit encounters to "unsafe" areas

    public GameObject battleCamera; //for transitioning into the battle camera
    public GameObject playerCamera;

    private bool willHaveEncounter = false;


    // TODO: change to don't destoy on load when we have extra game areas outside of the original and battle area
    void Awake()
    {
        if (Instance == null)
        {
            // set up game manager static instance
            Instance = this;
            // subscribe to battle end event
            BattleManager.BattleEndEvent += OnBattleEnd;
            PlayerMovement.StepCompleted += CheckForRandomEncounter;
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
                AudioManager.Instance.PlayMusic("OverworldMusic");
                willHaveEncounter = false;
                break;
            case GameState.Fighting:
                // Activate the BattleManager
                // move to battle scene
                TransitionToBattleFromOverworld();
                AudioManager.Instance.PlayMusic("BattleMusic");
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
                if (willHaveEncounter)
                {
                   // Debug.Log("Encounter");
                    UpdateGameState(GameState.Fighting);
                }
                break;
        }
    }

    public void CheckForRandomEncounter()
    {
        if (inSafeArea)
        {
            willHaveEncounter = false;
            return;
        }
        // if we haven't moved then don't trigger an encounter
        if (stepsTakenInOverworld < 10)
        {
            willHaveEncounter = false;
            return;
        }

        // use steps count to determine if we have encountered an enemy
        Debug.Log("Steps taken in overworld:" + (int)stepsTakenInOverworld);

        float res = UnityEngine.Random.Range(0f, 300f) - stepsTakenInOverworld;

        willHaveEncounter = res < 20; // TODO: replace this with the final random encounter formula
    }

    private void TransitionToBattleFromOverworld(bool bossFight = false)
    {
        // get a "hook" for the scene to return to on battle conclusion
        overworldScene = SceneManager.GetActiveScene();

        // set overworld return position
        overworldPosition = playergameObj.transform.position;

        // move necessary objects to battle scene
        SceneManager.MoveGameObjectToScene(playergameObj, SceneManager.GetSceneByName("BattleScene"));
        SceneManager.MoveGameObjectToScene(transform.parent.gameObject, SceneManager.GetSceneByName("BattleScene"));

        // if we have a boss move that to the scene
        var bInfo = BattleManager.Instance.GetBossInfo();

        if (bInfo.BossGameObject != null && !bInfo.isBossAPrefabObject)
        {
            SceneManager.MoveGameObjectToScene(bInfo.BossGameObject, SceneManager.GetSceneByName("BattleScene"));
        }

        // set active scene to battle
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("BattleScene"));
        // get battle root
        var root = GameObject.FindWithTag("BattleRootRef").GetComponent<RootReferenceHolder>(); // find root
        root.rootObject.SetActive(true);
        // disable overworld root
        GameObject.FindWithTag("OverworldRootRef").GetComponent<RootReferenceHolder>().rootObject.SetActive(false);

        //activating the battle camera and deactivating the player camera will automatically change the camera position and settings
        battleCamera.SetActive(true);
        playerCamera.SetActive(false);
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

        //activating the player camera and deactivating the battle camera will automatically change the camera position and settings
        battleCamera.SetActive(false);
        playerCamera.SetActive(true);

        // move player to original overworld position
        playergameObj.transform.position = overworldPosition;
    }

    // Called from UpdateBattleState BattleManager (State Victory and Defeat)
    private void OnBattleEnd()
    {
        // event handler for battle end
        // for now we just return to the overworld
        UpdateGameState(GameState.Wandering);
    }

    private void OnDestroy()
    {
        // maybe not necessary, but as mentioned by violet this is probably good practice
        // unsubscribe from all events
        BattleManager.BattleEndEvent -= OnBattleEnd;
    }

    public string GetOverworldSceneName()
    {
        if (overworldScene == null)
        {
            return "";
        }
        else
        {
            return overworldScene.name;
        }
    }
}

// Add more states as necessary, these are for actions that fundamentally change what happens in the game
public enum GameState
{
    GameStart,
    Wandering,
    Fighting,
    InDialogue,
    ViewingInventory
}
