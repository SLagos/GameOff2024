using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour, PlayerControls.IPlayerActions
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Animation Settings")]
    [SerializeField] private float animationBlendSpeed = 10f;
    [SerializeField] private float minimumMoveThreshold = 0.1f;
    
    [Header("References")]
    [SerializeField] private Animator animator;  // Reference to child's Animator
    
    private CharacterController controller;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        
        // Validate animator reference
        if (animator == null)
        {
            Debug.LogError("Animator reference not set on PlayerMovement!");
        }
        
        controls = new PlayerControls();
        controls.Player.SetCallbacks(this);
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
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // Implementation required by interface
    }

    private void Update()
    {
        HandleMovement();
        UpdateAnimations();
    }

    private void HandleMovement()
    {
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = transform.TransformDirection(moveDirection);
        
        // Add gravity
        moveDirection.y += Physics.gravity.y;
        
        controller.Move(moveDirection * (moveSpeed * Time.deltaTime));
    }

    private void UpdateAnimations()
    {
        // Convert world space movement to local space for animations
        Vector3 localMovement = transform.InverseTransformDirection(moveDirection);
        
        // Update animation parameters
        float moveX = localMovement.x;
        float moveZ = localMovement.z;
        
        // Smoothly interpolate current animation values to target values
        float currentMoveX = animator.GetFloat("MoveX");
        float currentMoveZ = animator.GetFloat("MoveZ");
        
        animator.SetFloat("MoveX", Mathf.Lerp(currentMoveX, moveX, Time.deltaTime * animationBlendSpeed));
        animator.SetFloat("MoveZ", Mathf.Lerp(currentMoveZ, moveZ, Time.deltaTime * animationBlendSpeed));
        
        // Set IsMoving parameter based on raw input magnitude
        bool isMoving = moveInput.magnitude > minimumMoveThreshold;
        Debug.Log($"Move Input Magnitude: {moveInput.magnitude}, IsMoving: {isMoving}");
        
        animator.SetBool("IsMoving", isMoving);
    }
} 