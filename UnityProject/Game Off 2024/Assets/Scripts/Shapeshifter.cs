using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Linq;

public class Shapeshifter : NetworkBehaviour, PlayerControls.IPlayerMimicActions
{
    [Header("Shape Settings")]
    [SerializeField] private MimicLibrary mimicLibrary;
    [SerializeField] private List<MimicsList> availableShapes = new List<MimicsList>();
    private MimicData currentShape;
    
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

        if (mimicLibrary == null)
        {
            Debug.LogError("MimicLibrary reference not set on Shapeshifter!");
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
        
        if (availableShapes.Count > 0)
        {
            currentShape = GetMimicDataFromEnum(availableShapes[0]);
        }
        
        ApplyCurrentShape();
    }

    private MimicData GetMimicDataFromEnum(MimicsList mimicType)
    {
        if (mimicType == MimicsList.None) return null;
        
        string mimicId = mimicType.ToString();
        MimicData mimicData = mimicLibrary.mimics.FirstOrDefault(m => m.id == mimicId);
        
        if (mimicData == null)
        {
            Debug.LogError($"No MimicData found for ID: {mimicId}");
        }
        
        return mimicData;
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
        currentShape = GetMimicDataFromEnum(availableShapes[currentShapeIndex]);
        
        ApplyCurrentShape();
    }

    private void ApplyCurrentShape()
    {
        if (currentShape == null) return;

        // Destroy previous shape instance and remove old collider
        if (currentShapeInstance != null)
        {
            var oldCollider = GetComponent<Collider>();
            if (oldCollider != null) Destroy(oldCollider);
            Destroy(currentShapeInstance);
        }

        // Instantiate new shape
        currentShapeInstance = Instantiate(currentShape.visualPrefab, visualParent);
        currentShapeInstance.transform.localPosition = Vector3.zero;
        currentShapeInstance.transform.localRotation = Quaternion.identity;

        // Copy collider from visual prefab to player
        var prefabCollider = currentShapeInstance.GetComponent<Collider>();
        if (prefabCollider != null)
        {
            var newCollider = gameObject.AddComponent(prefabCollider.GetType()) as Collider;
            if (newCollider != null)
            {
                // Copy all collider properties
                if (prefabCollider is BoxCollider)
                {
                    var boxSource = (BoxCollider)prefabCollider;
                    var boxTarget = (BoxCollider)newCollider;
                    boxTarget.center = boxSource.center;
                    boxTarget.size = boxSource.size;
                }
                else if (prefabCollider is CapsuleCollider)
                {
                    var capsuleSource = (CapsuleCollider)prefabCollider;
                    var capsuleTarget = (CapsuleCollider)newCollider;
                    capsuleTarget.center = capsuleSource.center;
                    capsuleTarget.radius = capsuleSource.radius;
                    capsuleTarget.height = capsuleSource.height;
                    capsuleTarget.direction = capsuleSource.direction;
                }
                // Add more collider types as needed
            }
        }

        // Update movement speed using the property
        playerMovement.MoveSpeed = currentShape.speed;
    }
} 