using System;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = nameof(StartingGameState), menuName = "GameStates/" + nameof(StartingGameState), order = 0)]
public class StartingGameState : AppState
{
    [SerializeField]
    private List<NetworkObject> prefabs;
    [SerializeField]
    private NetworkObject overridePrefab;
    private bool isGameSceneLoaded = false;
    private bool isConnectedToGame = false;
    private bool isPrefabInstantiated = false;
    public override EAppStateId Id { get => EAppStateId.StartingGame; }
    public override async void OnEnter()
    {
        _transitions = new List<AppStateTransition>
        {new AppStateTransition(EAppStateId.InGame, IsFinish )};

        isGameSceneLoaded = false;
        isConnectedToGame = false;
        isPrefabInstantiated = false;

        NetworkManager.Singleton.NetworkConfig.AutoSpawnPlayerPrefabClientSide = false;
        NetworkEventDispatcher.OnCLientStartedEvent += OnClientStarted;
        NetworkEventDispatcher.OnClientConnectedEvent += OnClientConnected;
        await SceneManager.LoadSceneAsync(1); //DungeonBlockout
        isGameSceneLoaded = true;

        if (GameManager.Instance.IsQuickPlay)
        {
            await RelayManager.Instance.CreateRelay();
        }

        if (LobbyManager.Instance.IsHost || GameManager.Instance.IsQuickPlay)
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

    private void OnClientConnected(ulong obj)
    {
        if (!NetworkManager.Singleton.IsHost) return;
        int prefabIndex = obj == NetworkManager.Singleton.LocalClientId ? 0 : 1;
        if (overridePrefab == null)
        {
            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(prefabs[prefabIndex], obj, isPlayerObject: true, forceOverride: true, position:Vector3.zero + Vector3.up);
        }
        else
        {
            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(overridePrefab, obj, isPlayerObject: true, forceOverride: true, position: Vector3.zero + Vector3.up);
        }
    }

    private void OnClientStarted()
    {
        // if (NetworkManager.Singleton.IsClient)
        // {
        //     var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        //     var virtualCamera = FindAnyObjectByType<CinemachineVirtualCamera>();

        //     virtualCamera.Follow = player.transform;
        //     //player.Spawn();
        //     isConnectedToGame = true;
        // }
        isConnectedToGame = true;
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