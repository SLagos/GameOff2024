
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MainMenuState), menuName = "GameStates/" + nameof(MainMenuState), order = 0)]
public class MainMenuState : AppState
{
    public override EAppStateId Id { get => EAppStateId.MainMenu; }

    public bool IsQuickStart = false;

    public override void OnEnter()
    {
        _transitions = new List<AppStateTransition>
        {
            new AppStateTransition(EAppStateId.LookingForLobby, IsLookingForLobby ),
            new AppStateTransition(EAppStateId.StartingGame, IsQuickPlay ),
        };
        UIManager.Instance.ShowMainMenu();
        GameManager.Instance.SetQuickStart(IsQuickStart);

    }

    private bool IsLookingForLobby()
    {
        return !IsQuickStart;
    }

    private bool IsQuickPlay()
    {
        return IsQuickStart;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
    }
}