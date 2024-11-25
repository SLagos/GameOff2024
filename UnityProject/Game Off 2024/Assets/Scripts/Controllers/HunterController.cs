using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FieldOfView), typeof(PlayerMovement))]
public class HunterController : NetworkBehaviour, PlayerControls.IPlayerActions
{
    [SerializeField] private float minDistanceToInteract = 2f;

    private FieldOfView fov;
    private List<TargetInfo> targetsInReach = new List<TargetInfo>();
    private IInteractable closestInteractable;
    private PlayerControls controls;
    private PlayerMovement movementController;

    private bool isInteracting = false;
    private Coroutine interactionCoroutine;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        if (closestInteractable != null && !isInteracting)
        {
            StartInteraction();
        }
    }

    private void StartInteraction()
    {
        if (closestInteractable != null)
        {
            movementController.CanMove = false;
            isInteracting = true;
            interactionCoroutine = StartCoroutine(StartInteractionCoroutine(closestInteractable.InteractionTime));
            UIManager.Instance.HideInteractText();
        }
    }

    private void Interact()
    {
        if (closestInteractable != null)
        {
            closestInteractable.Interact();
        }
    
        EndInteraction();
    }

    private void EndInteraction()
    {
        movementController.CanMove = true;
        isInteracting = false;
        UIManager.Instance.SetInterctionDisplay(false,Vector3.zero);
    }

    private IEnumerator StartInteractionCoroutine(float time)
    {
        float timePassed = 0f;
        float t = timePassed / time;
        UIManager.Instance.SetInteractionTime(time, t);
        UIManager.Instance.SetInterctionDisplay(true, closestInteractable.Position);

        while (timePassed < time)
        {
            timePassed += Time.deltaTime;
            t = timePassed / time;
            UIManager.Instance.SetInteractionTime(time - timePassed, t);
            yield return null;
        }
        Interact();
    }



    public void OnLook(InputAction.CallbackContext context)
    {

    }

    public void OnMove(InputAction.CallbackContext context)
    {

    }

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.SetCallbacks(this);
        movementController = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        fov = GetComponent<FieldOfView>();
        controls = new PlayerControls();
        controls.Player.SetCallbacks(this);
    }

    void Update()
    {
        if (!IsOwner || isInteracting) return;
        targetsInReach.Clear();
        var targets = fov.visibleTargets;
        foreach (var target in targets)
        {
            if (target.transform == null) continue; //We skip this transform since was destroyed
            var interactable = target.transform.GetComponent<IInteractable>();
            if (target.distance <= minDistanceToInteract && interactable != null)
            {
                targetsInReach.Add(target);
            }
        }
        targetsInReach.Sort((a, b) => a.distance.CompareTo(b.distance));

        if (targetsInReach.Count > 0)
        {
            var closestTarget = targetsInReach[0].transform;
            closestInteractable = closestTarget.GetComponent<IInteractable>();
            UIManager.Instance.SetInteractText("Press E to interact", closestTarget.position);
        }
        else
        {
            UIManager.Instance.HideInteractText();
            closestInteractable = null;
        }
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (interactionCoroutine != null)
        {
            StopCoroutine(interactionCoroutine);
        }
    }
}
