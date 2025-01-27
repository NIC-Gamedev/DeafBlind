using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private Vector2 mouseSensitivity = new Vector2(8,0.5f); 

    private Vector2 cameraRotation;

    private Vector2 mouseAxis;

    private MainController inputActions;

    [SerializeField] private Transform body;

    public override void OnNetworkSpawn()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        InputInit();
        if(!IsOwner)
        {
            this.enabled = false;
        }
    }
 

    private void InputInit()
    {
        inputActions = InputManager.inputActions;
        inputActions.Player.Look.Enable();
        inputActions.Player.Look.performed += RotateCamera;
    }

    private void RotateCamera(InputAction.CallbackContext callback)
    {
        mouseAxis = callback.ReadValue<Vector2>() * mouseSensitivity;
        cameraRotation.y += mouseAxis.x;
        cameraRotation.x -= mouseAxis.y;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90f, 90f);
        transform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);
        body.rotation = Quaternion.Euler(0, cameraRotation.y, 0);
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }
}
