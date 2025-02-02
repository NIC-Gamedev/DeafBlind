using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FishNet;
using FishNet.Object;

public class PlayerCamera : NetworkBehaviour
{
 
    [SerializeField] private Vector2 mouseSensitivity = new Vector2(8, 0.5f);

    [SerializeField] [Range(0,90f)]private float angleY = 90; 


    private Vector2 cameraRotation;

    private Vector2 mouseAxis;

    private MainController inputActions;

    [SerializeField] private Transform body;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        InputInit();
    }

    public override void OnStartClient()
    {
        if(IsOwner)
        {

        }
        else
        {
            this.gameObject.SetActive(false);
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
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -angleY, angleY);
        transform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);
        body.rotation = Quaternion.Euler(0, cameraRotation.y, 0);
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }
}