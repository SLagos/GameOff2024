using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = nameof(InGameState), menuName = "GameStates/" + nameof(InGameState), order = 0)]
public class InGameState : AppState
{

    public override EAppStateId Id { get => EAppStateId.InGame; }
    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
    }
}