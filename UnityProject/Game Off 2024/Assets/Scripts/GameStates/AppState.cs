using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AppState : ScriptableObject
{
    public abstract EAppStateId Id { get; }
    public bool IsInitialized { get; protected set; } = false;
    public bool IsExiting { get; protected set; } = false;

    protected List<AppStateTransition> _transitions = new List<AppStateTransition>();

    public void Enter()
    {
        OnEnter();
        IsInitialized = true;
    }

    public void Exit()
    {
        IsExiting = true;
        OnExit();
        IsExiting = false;
    }

    public void Update()
    {
        OnUpdate();
        foreach (var transition in _transitions)
        {
            if(transition.IsConditionMet())
            {
                GameManager.Instance.ChangeState(transition.To);
                break;
            }
        }
    }
    public virtual void OnEnter(){}
    public virtual void OnExit(){}

    public virtual void OnUpdate(){}
}

public class AppStateTransition
{
    public EAppStateId To { get; private set; }
    private Func<bool> _isConditionMet;

    public AppStateTransition(EAppStateId to, Func<bool> isConditionMet)
    {
        To = to;
        _isConditionMet = isConditionMet;
    }

    public bool IsConditionMet()
    {
        return _isConditionMet();
    }
}
