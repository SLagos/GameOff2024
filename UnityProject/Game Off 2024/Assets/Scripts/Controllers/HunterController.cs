using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FieldOfView))]
public class HunterController : MonoBehaviour, PlayerControls.IPlayerActions
{
    [SerializeField] private float minDistanceToInteract = 2f;

    private FieldOfView fov;
    private List<TargetInfo> targetsInReach = new List<TargetInfo>();
    private IInteractable closestInteractable;
    private PlayerControls controls;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (closestInteractable != null)
        {
            closestInteractable.Interact();
        }
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
    }

    private void Start()
    {
        fov = GetComponent<FieldOfView>();
        controls = new PlayerControls();
        controls.Player.SetCallbacks(this);
    }

    void Update()
    {
        targetsInReach.Clear();
        var targets = fov.visibleTargets;
        foreach (var target in targets)
        {
            if(target.transform == null) continue; //We skip this transform since was destroyed
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
}
