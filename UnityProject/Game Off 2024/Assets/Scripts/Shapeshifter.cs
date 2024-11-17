using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Linq;
using Cinemachine;
using UnityEditor.SceneManagement;

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
    private ClientNetworkAnimator clientNetworkAnimator;

    private CinemachineVirtualCamera virtualCamera;

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
        //if (!IsOwner) return;

        if (availableShapes.Count > 0)
        {
            currentShape = GetMimicDataFromEnum(availableShapes[0]);
        }

        if (IsServer)
            ApplyCurrentShapeServer(currentShapeIndex);
        else
            ApplyCurrentShapeServerRpc(currentShapeIndex);
    }

    protected override void OnNetworkPostSpawn()
    {
        if (!IsOwner) return;
        virtualCamera = FindAnyObjectByType<CinemachineVirtualCamera>();
        virtualCamera.Follow = transform;
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
        if (IsServer)
            ApplyCurrentShapeServer(currentShapeIndex);
        else
            ApplyCurrentShapeServerRpc(currentShapeIndex);

    }
    [ServerRpc]
    private void ApplyCurrentShapeServerRpc(int shapeIndex)
    {
        ApplyCurrentShapeServer(shapeIndex);
    }

    private void ApplyCurrentShapeServer(int shapeIndex)
    {
        currentShapeIndex = shapeIndex;
        currentShape = GetMimicDataFromEnum(availableShapes[currentShapeIndex]);
        if (currentShape == null) return;
        

        // Destroy previous shape instance and remove old collider
        if (currentShapeInstance != null)
        {
            currentShapeInstance.GetComponent<NetworkObject>().Despawn();
            var oldCollider = GetComponent<Collider>();
            if (oldCollider != null) Destroy(oldCollider);
            Destroy(currentShapeInstance);
        }

        // Instantiate new shape
        var networkObject =  Instantiate(currentShape.visualPrefab, visualParent).GetComponent<NetworkObject>();
        //NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(currentShape.visualPrefab.GetComponent<NetworkObject>(), OwnerClientId, isPlayerObject: true, forceOverride: true);
        
        currentShapeInstance = networkObject.gameObject;
        currentShapeInstance.transform.localPosition = Vector3.zero;
        currentShapeInstance.transform.localRotation = Quaternion.identity;

        //Copy collider from visual prefab to player
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

        networkObject.SpawnWithOwnership(OwnerClientId);
        networkObject.TrySetParent(visualParent);
        

        // var componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        // if (componentBase is Cinemachine3rdPersonFollow)
        // {
        //     (componentBase as Cinemachine3rdPersonFollow).CameraDistance = currentShape.cameraDistance;
        //     (componentBase as Cinemachine3rdPersonFollow).VerticalArmLength = currentShape.camerHeight;
        // }

        // var animator = currentShapeInstance.GetComponent<Animator>();
        // playerMovement.SetAnimator(animator);

        // if (clientNetworkAnimator == null)
        // {
        //     clientNetworkAnimator = gameObject.AddComponent<ClientNetworkAnimator>();
        // }
        // clientNetworkAnimator.Animator = animator;

        // Update movement speed using the property
        playerMovement.MoveSpeed = currentShape.speed;
        SetupNewFollowTargetClientRpc();
    }

    [ClientRpc]
    private void SetupNewFollowTargetClientRpc()
    {
        if(!IsOwner) return;
        playerMovement.SetFollowTarget(transform);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
    }
}