
using UnityEngine;

public interface IInteractable
{
    public float InteractionTime { get; }
    public Vector3 Position { get; }
    public EInteractableType InteractableType { get; }
    public void Interact();
}

public enum EInteractableType{
    None = 0,
    Interactable = 1

}
