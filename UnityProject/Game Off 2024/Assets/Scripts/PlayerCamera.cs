using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using Unity.Netcode;

public class PlayerCamera : NetworkBehaviour, PlayerControls.IPlayerActions
{
    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivityX = 2f;
    [SerializeField] private float mouseSensitivityY = 2f;
    [SerializeField] private float maxLookUpAngle = 80f;
    [SerializeField] private float minLookUpAngle = -80f;
    
    [Header("References")]
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    
    private PlayerControls controls;
    private Vector2 lookInput;
    private float cameraPitch;
    
    private void Awake()
    {        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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
        // We don't need to implement anything here since this script only handles camera
    }

    public void OnLook(InputAction.CallbackContext context)
    {        
        lookInput = context.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        if(!IsOwner && virtualCamera.gameObject.activeSelf)
        {
            virtualCamera.gameObject.SetActive(false);
            return;
        } 
        if (lookInput != Vector2.zero)
        {
            // Handle horizontal rotation (player rotation)
            float horizontalRotation = lookInput.x * mouseSensitivityX;
            transform.Rotate(Vector3.up, horizontalRotation);
            
            // Handle vertical rotation (camera pitch)
            cameraPitch -= lookInput.y * mouseSensitivityY;
            cameraPitch = Mathf.Clamp(cameraPitch, minLookUpAngle, maxLookUpAngle);
            
            // Apply pitch to camera root
            cameraRoot.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if(!IsOwner) return;
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
} 