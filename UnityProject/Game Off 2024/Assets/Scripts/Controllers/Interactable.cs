using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{

    public EInteractableType InteractableType => EInteractableType.Interactable;

    public void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);
    }
}