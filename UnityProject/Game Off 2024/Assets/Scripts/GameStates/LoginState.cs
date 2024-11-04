using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Services.Authentication;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(LoginState), menuName = "GameStates/" + nameof(LoginState), order = 0)]
public class LoginState : AppState
{

    public override EAppStateId Id { get => EAppStateId.Login; }
    public override void OnEnter()
    {
        //AuthenticationService.Instance.SignedIn += OnSignedIn;
        _transitions = new List<AppStateTransition>
        {new AppStateTransition(EAppStateId.MainMenu, IsSignedIn )};
        OnLogIn("TestUser");
    }

    private bool IsSignedIn()
    {
        return AuthenticationService.Instance.IsSignedIn;
    }

    public override void OnExit()
    {
        //AuthenticationService.Instance.SignedIn -= OnSignedIn;
    }

    public override void OnUpdate()
    {
    }

    private async void OnLogIn(string userName)
    {
        //AuthenticationService.Instance.SwitchProfile(userName);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    private void OnSignedIn()
    {

    }
}