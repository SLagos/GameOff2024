using System;
using Cinemachine;
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
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private LayerMask groundMask;

    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivityX = 2f;
    [SerializeField] private float mouseSensitivityY = 2f;
    [SerializeField] private float maxLookUpAngle = 80f;
    [SerializeField] private float minLookUpAngle = -80f;

    [SerializeField] private Transform followTarget;

    [Header("Animation Settings")]
    [SerializeField] private float animationBlendSpeed = 10f;
    [SerializeField] private float minimumMoveThreshold = 0.1f;

    [Header("References")]
    [SerializeField] private Animator animator;

    private Camera playerCamera;
    private Rigidbody rb;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector3 moveDirection;

    private Vector2 lookInput;

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

        // if (animator == null)
        // {
        //     Debug.LogError("Animator reference not set on PlayerMovement!");
        // }

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
        if (!IsOwner) return;
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (!IsOwner) return;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        HandleMovement();
        //HandleRotation();

    }

    private void HandleRotation()
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void HandleMovement()
    {
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        // Project these vectors onto the horizontal plane
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate the move direction in world space
        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        // Rotate the character to match the camera's forward direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }


        // Apply movement force
        if (moveDirection.magnitude > 0)
        {
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

        animator.SetFloat(MoveXHash, moveInput.x);
        animator.SetFloat(MoveZHash, moveInput.y);

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

    public void SetAnimator(Animator newAnimator)
    {
        animator = newAnimator;
    }

    public void SetFollowTarget(Transform newFollowTarget)
    {
        followTarget = newFollowTarget;
        SetFollowTarget();
    }

    protected override void OnNetworkPostSpawn()
    {
        if (!IsOwner) return;
        playerCamera = Camera.main;
        SetFollowTarget();

    }

    private void SetFollowTarget()
    {
        var virtualCamera = FindAnyObjectByType<CinemachineFreeLook>();
        virtualCamera.Follow = followTarget;
        virtualCamera.LookAt = followTarget;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (IsOwner) return;
        if (hasFocus) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }
}