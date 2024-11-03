

using UnityEngine;

public class FSM
{
    public AppState CurrentState { get; private set; }
    public void ChangeState(AppState newState)
    {
        Debug.Log($"Changing state from {CurrentState?.Id} to {newState?.Id}");
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        if(CurrentState != null 
        && CurrentState.IsInitialized
        && !CurrentState.IsExiting){
            CurrentState.Update();
        }
    }


}