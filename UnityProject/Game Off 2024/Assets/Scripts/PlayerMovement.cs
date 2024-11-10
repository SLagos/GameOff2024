using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour, PlayerControls.IPlayerActions
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float groundDrag = 15f;
    [SerializeField] private float airDrag = 1f;
    [SerializeField] private float movementForce = 70f;
    [SerializeField] private float stopForceMultiplier = 3f;
    [SerializeField] private float stopThreshold = 0.1f;
    [SerializeField] private LayerMask groundMask;
    
    [Header("Animation Settings")]
    [SerializeField] private float animationBlendSpeed = 10f;
    [SerializeField] private float minimumMoveThreshold = 0.1f;
    
    [Header("References")]
    [SerializeField] private Animator animator;
    
    private Rigidbody rb;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    
    // Animation parameter IDs
    private readonly int MoveXHash = Animator.StringToHash("MoveX");
    private readonly int MoveZHash = Animator.StringToHash("MoveZ");
    private readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    
    private bool isGrounded;
    private float groundCheckDistance = 0.2f;
    
    private Collider activeCollider;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent rigidbody from rotating
        
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
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;
        HandleMovement();
    }

    private void HandleMovement()
    {
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = transform.TransformDirection(moveDirection);
        
        if (moveDirection.magnitude > 0)
        {
            moveDirection.Normalize();
            // Apply movement force
            rb.AddForce(moveDirection * moveSpeed * movementForce, ForceMode.Force);
        }
        else if (isGrounded)
        {
            // Get horizontal velocity only
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            
            if (horizontalVelocity.magnitude < stopThreshold)
            {
                // If moving very slowly, stop horizontal movement completely
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
            else
            {
                // Apply a strong opposing force to stop quickly
                Vector3 oppositeForce = -horizontalVelocity.normalized * 
                                       moveSpeed * 
                                       movementForce * 
                                       stopForceMultiplier;
                
                rb.AddForce(oppositeForce, ForceMode.Impulse);
            }
        }

        // Apply drag
        rb.linearDamping = isGrounded ? groundDrag : airDrag;

        // Limit velocity
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void UpdateAnimations()
    {
        // Early return if no animator is assigned
        if (animator == null) return;

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

    public void SetCollider(Collider newCollider)
    {
        if (activeCollider != null)
        {
            activeCollider.enabled = false;
        }
        
        activeCollider = newCollider;
        activeCollider.enabled = true;
    }

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }
} 