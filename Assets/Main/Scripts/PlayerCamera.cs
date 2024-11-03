using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Vector2 mouseSensitivity = new Vector2(8,0.5f); 

    private Vector2 cameraRotation;

    private Vector2 mouseAxis;

    private MainController inputActions;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        InputInit();
    }

    private void InputInit()
    {
        inputActions = new MainController();
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
        transform.parent.rotation = Quaternion.Euler(0, cameraRotation.y, 0);
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }
}
