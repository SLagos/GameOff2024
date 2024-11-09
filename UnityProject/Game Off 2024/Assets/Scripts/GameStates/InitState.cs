
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(InitState), menuName = "GameStates/" + nameof(InitState), order = 0)]
public class InitState : AppState
{
    public override EAppStateId Id { get => EAppStateId.Init; }

    private bool IsInitializationFinished()
    {
       return IsInitialized;
    }

    public override async void OnEnter()
    {
        _transitions = new List<AppStateTransition>
        {new AppStateTransition(EAppStateId.Login, IsInitializationFinished )};

        try
        {
            Debug.Log("Initializing Unity Services...");
            UnityServices.Initialized += () =>
            {
                IsInitialized = true;
                Debug.Log("Unity Services initialized.");
            };
            UnityServices.InitializeFailed += (e) => { Debug.Log(e); };
            await UnityServices.InitializeAsync();
            NetworkEventDispatcher.StartListeningNetworkEvents();
        }

        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
    }
}
