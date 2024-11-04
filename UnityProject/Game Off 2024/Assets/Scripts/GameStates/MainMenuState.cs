
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MainMenuState), menuName = "GameStates/" + nameof(MainMenuState), order = 0)]
public class MainMenuState : AppState
{
    public override EAppStateId Id { get => EAppStateId.MainMenu; }

    public override void OnEnter()
    {
        _transitions = new List<AppStateTransition>
        {new AppStateTransition(EAppStateId.LookingForLobby, IsLookingForLobby )};
        UIManager.Instance.ShowMainMenu();

    }

    private bool IsLookingForLobby()
    {
        return true;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
    }
}