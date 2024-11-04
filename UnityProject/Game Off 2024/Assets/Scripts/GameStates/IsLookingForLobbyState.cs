using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(IsLookingForLobbyState), menuName = "GameStates/" + nameof(IsLookingForLobbyState), order = 0)]

public class IsLookingForLobbyState : AppState
{
    public override EAppStateId Id => EAppStateId.LookingForLobby;

    public override void OnEnter()
    {
         _transitions = new List<AppStateTransition>
        {new AppStateTransition(EAppStateId.InLobby, IsInLobby )};
        UIManager.Instance.GotoLobbiesView();
    }

    private bool IsInLobby()
    {
        return LobbyManager.Instance.IsInLobby;
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
    }
}