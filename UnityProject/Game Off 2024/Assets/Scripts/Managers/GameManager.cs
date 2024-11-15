using System.Collections.Generic;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private FSM appState = new FSM();

    private Dictionary<EAppStateId,AppState> appStates = new Dictionary<EAppStateId,AppState>();

    public bool IsQuickPlay { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        LoadAppStates();
        AppState initState = appStates[EAppStateId.Init];
        appState.ChangeState(initState);
    }

    private void LoadAppStates()
    {
        // Load all AppState objects from the Resources folder
        AppState[] loadedStates = Resources.LoadAll<AppState>("States");

        foreach (AppState state in loadedStates)
        {
            appStates.Add(state.Id, state);
        }

        Debug.Log($"Loaded {appStates.Count} AppState objects.");
    }

    public void ChangeState(EAppStateId newStateId)
    {
        appState.ChangeState(appStates[newStateId]);
    }

    void Update()
    {
        appState?.Update();
    }

    public void SetQuickStart(bool isQuickPlay)
    {
        IsQuickPlay = isQuickPlay;
    }



    public void Quit()
    {
        NetworkEventDispatcher.StopListeningNetworkEvents();
        Application.Quit();
    }

    
}