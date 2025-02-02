using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FishNet;
using FishNet.Object;
using FishNet.Managing;
using FishNet.Connection;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private Vector2 mouseSensitivity = new Vector2(8, 0.5f);

    [SerializeField][Range(0, 90f)] private float angleY = 90;

    private Vector2 cameraRotation;

    private Vector2 mouseAxis;

    private MainController inputActions;

    [SerializeField] private Transform body;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public override void OnStartClient()
    {
        if (IsOwner)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            virtualCamera.Priority += 1;
            InputInit();
        }
        else
        {
            virtualCamera.Priority = 0; // Сделать камеру неактивной для других игроков
        }
    }

    private void InputInit()
    {
        inputActions = InputManager.inputActions;
        inputActions.Player.Look.Enable();
        inputActions.Player.Look.performed += OnRotateCamera;
    }

    private void OnRotateCamera(InputAction.CallbackContext callback)
    {
        mouseAxis = callback.ReadValue<Vector2>() * mouseSensitivity;
        CameraRotation(mouseAxis);
    }

    public void CameraRotation(Vector2 mouseAxis)
    {
        cameraRotation.y += mouseAxis.x;
        cameraRotation.x -= mouseAxis.y;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -angleY, angleY);

        if (IsOwner)
        {
            // Обновляем камеру только на стороне клиента
            transform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);
            body.rotation = Quaternion.Euler(0, cameraRotation.y, 0);
        }
        else
        {
            // Синхронизируем с другими клиентами через ClientRpc
            UpdateCameraRotationClientRpc();
        }
    }

    // Теперь добавляем NetworkConnection как первый параметр в ClientRpc
    [TargetRpc]
    public void UpdateCameraRotationClientRpc(NetworkConnection conn = null)
    {
        // Это будет выполнено на всех клиентах
        transform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);
        body.rotation = Quaternion.Euler(0, cameraRotation.y, 0);
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }
}
