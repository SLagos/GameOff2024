using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = nameof(InGameState), menuName = "GameStates/" + nameof(InGameState), order = 0)]
public class InGameState : AppState
{

    public override EAppStateId Id { get => EAppStateId.InGame; }
    public override void OnEnter()
    {
        NetworkEventDispatcher.OnClientDisconnectedEvent += OnClientDisconnected;
    }

    private void OnClientDisconnected(ulong obj)
    {
        if(obj == NetworkManager.Singleton.LocalClientId && !NetworkManager.Singleton.ShutdownInProgress)
        {
            //This client was disconnected, will try to reconnect once
            //Rework eventually
            NetworkManager.Singleton.StartClient();
        }
    }

    public override void OnExit()
    {
        NetworkEventDispatcher.OnClientDisconnectedEvent -= OnClientDisconnected;
    }

    public override void OnUpdate()
    {
    }
}