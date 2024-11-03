using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour, PlayerControls.IPlayerActions
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    
    private CharacterController controller;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    
    private void Awake()
    {
        Debug.Log("PlayerMovement Awake");
        controller = GetComponent<CharacterController>();
        
        controls = new PlayerControls();
        controls.Player.SetCallbacks(this);
        Debug.Log("Controls initialized and callbacks set");
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        Debug.Log("Controls enabled");
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    // Implement the interface methods
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Debug.Log($"OnMove called: {moveInput}, Phase: {context.phase}");
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // Implement if needed
    }

    private void Update()
    {
        if (moveInput != Vector2.zero)
        {
            Debug.Log($"Applying movement: {moveInput}");
        }
        
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = transform.TransformDirection(moveDirection);
        
        // Add gravity
        moveDirection.y += Physics.gravity.y;
        
        controller.Move(moveDirection * (moveSpeed * Time.deltaTime));
    }
} 