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
        NetworkEventDispatcher.OnCLientStoppedEvent += OnClientStopped;
    }

    private void OnClientStopped(bool obj)
    {
        NetworkManager.Singleton.StartClient();
    }

    public override void OnExit()
    {
        NetworkEventDispatcher.OnCLientStoppedEvent -= OnClientStopped;
    }

    public override void OnUpdate()
    {
    }
}