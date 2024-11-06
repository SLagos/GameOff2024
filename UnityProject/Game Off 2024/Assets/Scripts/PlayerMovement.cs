using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour, PlayerControls.IPlayerActions
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Animation Settings")]
    [SerializeField] private float animationBlendSpeed = 10f;
    [SerializeField] private float minimumMoveThreshold = 0.1f;
    
    [Header("References")]
    [SerializeField] private Animator animator;
    
    private CharacterController controller;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    
    // Animation parameter IDs
    private readonly int MoveXHash = Animator.StringToHash("MoveX");
    private readonly int MoveZHash = Animator.StringToHash("MoveZ");
    private readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        
        if (animator == null)
        {
            Debug.LogError("Animator reference not set on PlayerMovement!");
        }
        
        controls = new PlayerControls();
        controls.Player.SetCallbacks(this);
        
        // Uncomment for debugging animation parameters
        // ValidateAnimatorParameters();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(!IsOwner) return;
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // Implementation required by interface
    }

    private void Update()
    {
        if(!IsOwner) return;
        HandleMovement();
        UpdateAnimations();
    }

    private void HandleMovement()
    {
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = transform.TransformDirection(moveDirection);
        
        moveDirection.y += Physics.gravity.y;
        controller.Move(moveDirection * (moveSpeed * Time.deltaTime));
    }

    private void UpdateAnimations()
    {
        Vector3 localMovement = transform.InverseTransformDirection(moveDirection);
        
        float moveX = localMovement.x;
        float moveZ = localMovement.z;
        
        // Smoothly interpolate current animation values to target values
        float currentMoveX = animator.GetFloat(MoveXHash);
        float currentMoveZ = animator.GetFloat(MoveZHash);
        
        animator.SetFloat(MoveXHash, Mathf.Lerp(currentMoveX, moveX, Time.deltaTime * animationBlendSpeed));
        animator.SetFloat(MoveZHash, Mathf.Lerp(currentMoveZ, moveZ, Time.deltaTime * animationBlendSpeed));
        
        // Set IsMoving parameter based on raw input magnitude instead of moveDirection
        bool isMoving = moveInput.magnitude > minimumMoveThreshold;
        animator.SetBool(IsMovingHash, isMoving);
    }

    // Kept for future debugging if needed
    private void ValidateAnimatorParameters()
    {
        bool foundMoveX = false;
        bool foundMoveZ = false;
        bool foundIsMoving = false;
        
        // Log all parameters and check if ours exist
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            Debug.Log($"Found parameter: {param.name}, type: {param.type}");
            
            if (param.nameHash == MoveXHash) foundMoveX = true;
            if (param.nameHash == MoveZHash) foundMoveZ = true;
            if (param.nameHash == IsMovingHash) foundIsMoving = true;
        }
        
        // Report any missing parameters
        if (!foundMoveX)
            Debug.LogError("MoveX parameter not found in Animator!");
        if (!foundMoveZ)
            Debug.LogError("MoveZ parameter not found in Animator!");
        if (!foundIsMoving)
            Debug.LogError("IsMoving parameter not found in Animator!");
    }
} 