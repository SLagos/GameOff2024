using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = nameof(StartingGameState), menuName = "GameStates/" + nameof(StartingGameState), order = 0)]
public class StartingGameState : AppState
{
    private bool isGameSceneLoaded = false;
    private bool isConnectedToGame = false;
    public override EAppStateId Id { get => EAppStateId.StartingGame; }
    public override async void OnEnter()
    {
        _transitions = new List<AppStateTransition>
        {new AppStateTransition(EAppStateId.InGame, IsFinish )};

        isGameSceneLoaded = false;
        isConnectedToGame = false;

        NetworkManager.Singleton.NetworkConfig.AutoSpawnPlayerPrefabClientSide = false;
        NetworkEventDispatcher.OnCLientStartedEvent += OnClientStarted;
        await SceneManager.LoadSceneAsync(1); //DungeonBlockout
        isGameSceneLoaded = true;

        if (LobbyManager.Instance.IsHost)
        {
            Debug.Log("Starting host");
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            Debug.Log("Starting client");
            NetworkManager.Singleton.StartClient();
        }

    }

    private void OnClientStarted()
    {
        if(NetworkManager.Singleton.IsClient)
        {
            var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            //player.Spawn();
            isConnectedToGame = true;
        }
    }

    private bool IsFinish()
    {
        return isGameSceneLoaded
        && isConnectedToGame;
    }

    public override void OnExit()
    {
        UIManager.Instance.HideMainMenu();
    }

    public override void OnUpdate()
    {

    }

}