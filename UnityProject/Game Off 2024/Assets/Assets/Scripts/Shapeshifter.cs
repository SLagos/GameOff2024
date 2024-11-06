using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class Shapeshifter : NetworkBehaviour, PlayerControls.IPlayerMimicActions
{
    [Header("Shape Settings")]
    [SerializeField] private MimicData currentShape;
    [SerializeField] private List<MimicData> availableShapes = new List<MimicData>();
    
    [Header("References")]
    [SerializeField] private Transform visualParent;
    
    private PlayerMovement playerMovement;
    private GameObject currentShapeInstance;
    private int currentShapeIndex = 0;
    private PlayerControls controls;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on Shapeshifter GameObject!");
        }

        controls = new PlayerControls();
        controls.PlayerMimic.SetCallbacks(this);
    }

    private void OnEnable()
    {
        controls.PlayerMimic.Enable();
    }

    private void OnDisable()
    {
        controls.PlayerMimic.Disable();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        
        if (currentShape == null && availableShapes.Count > 0)
        {
            currentShape = availableShapes[0];
        }
        
        ApplyCurrentShape();
    }

    public void OnShapeshift(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        
        if (context.performed)
        {
            CycleToNextShape();
        }
    }

    private void CycleToNextShape()
    {
        if (availableShapes.Count == 0) return;

        currentShapeIndex = (currentShapeIndex + 1) % availableShapes.Count;
        currentShape = availableShapes[currentShapeIndex];
        
        ApplyCurrentShape();
    }

    private void ApplyCurrentShape()
    {
        if (currentShape == null) return;

        // Destroy previous shape instance if it exists
        if (currentShapeInstance != null)
        {
            Destroy(currentShapeInstance);
        }

        // Instantiate new shape
        currentShapeInstance = Instantiate(currentShape.visualPrefab, visualParent);
        currentShapeInstance.transform.localPosition = Vector3.zero;
        currentShapeInstance.transform.localRotation = Quaternion.identity;

        // Update movement speed
        playerMovement.MoveSpeed = currentShape.speed;
    }
} 