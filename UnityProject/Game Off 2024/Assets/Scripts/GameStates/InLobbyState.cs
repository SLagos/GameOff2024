using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(InLobbyState), menuName = "GameStates/" + nameof(InLobbyState), order = 0)]

public class InLobbyState : AppState
{
    public override EAppStateId Id => EAppStateId.InLobby;

    private bool isGameDataReady = false;

    public override void OnEnter()
    {
        isGameDataReady = false;
        _transitions = new List<AppStateTransition>
        {
            new AppStateTransition(EAppStateId.StartingGame, IsGameDataReady ),
            new AppStateTransition(EAppStateId.LookingForLobby, ()=> !IsInLobby())
        };
        RelayManager.Instance.OnRelayDataSet += OnRelayDataSet;
        UIManager.Instance.GoToLobbyView();
    }

    private void OnRelayDataSet()
    {
        RelayManager.Instance.OnRelayDataSet -= OnRelayDataSet;
        isGameDataReady = true;
    }

    private bool IsInLobby()
    {
        return LobbyManager.Instance.IsInLobby;
    }

    private bool IsGameDataReady()
    {
        return isGameDataReady;
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
    }
}