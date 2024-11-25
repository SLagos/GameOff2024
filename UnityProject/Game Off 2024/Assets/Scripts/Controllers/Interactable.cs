using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] private float interactionTime;

    public EInteractableType InteractableType => EInteractableType.Interactable;

    public float InteractionTime => interactionTime;

    public Vector3 Position => transform.position;

    public void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);
    }
}