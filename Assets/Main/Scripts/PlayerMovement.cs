using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MovementBase
{
    MainControll inputActions;
    private Vector3 input;
    protected override void Awake()
    {
        base.Awake();
        inputActions = new MainControll();
        inputActions.Enable();

        inputActions.Player.Movement.performed += callback => input = callback.ReadValue<Vector3>();
        inputActions.Player.Movement.canceled += callback => input = callback.ReadValue<Vector3>();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    protected override void Movement()
    {
        Vector3 direction =  transform.forward * input.y + transform.right * input.x ;
        if (direction == Vector3.zero)
            return;

        rb.MovePosition(transform.position + direction.normalized * movementSpeed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }
}
