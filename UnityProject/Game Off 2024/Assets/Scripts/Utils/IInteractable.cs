public interface IInteractable
{
    public EInteractableType InteractableType { get; }
    public void Interact();
}

public enum EInteractableType{
    None = 0,
    Interactable = 1

}
