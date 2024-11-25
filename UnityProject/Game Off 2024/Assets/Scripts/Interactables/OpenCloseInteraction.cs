using Unity.VisualScripting;
using UnityEngine;

public class OpenCloseInteraction : Interactable
{
    [SerializeField] private bool isOpen = false;
    [SerializeField] private GameObject openObject;
    
    public override void Interact()
    {
        base.Interact();
        isOpen = !isOpen;
        openObject.SetActive(isOpen);
    }
}