using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(InLobbyState), menuName = "GameStates/" + nameof(InLobbyState), order = 0)]

public class InLobbyState : AppState
{
    public override EAppStateId Id => EAppStateId.InLobby;

    private bool isGameStarted = false;

    public override void OnEnter()
    {
        _transitions = new List<AppStateTransition>
        {
            new AppStateTransition(EAppStateId.InGame, IsGameStarted ),
            new AppStateTransition(EAppStateId.LookingForLobby, ()=> !IsInLobby())
        };
        UIManager.Instance.GoToLobbyView();
        NetworkManager.Singleton.OnClientStarted += OnClientConnected;
    }

    private bool IsInLobby()
    {
        return LobbyManager.Instance.IsInLobby;
    }

    private void OnClientConnected()
    {
        isGameStarted = true;
    }

    private bool IsGameStarted()
    {
        return isGameStarted;
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
    }
}