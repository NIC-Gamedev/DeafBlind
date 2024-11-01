using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MovementBase
{
    MainControll inputActions;
    private void Awake()
    {
        inputActions = new MainControll();
        inputActions.Enable();
    }
    protected void Movement(Vector3 direction)
    {
       Vector3 direction2 = inputActions.PlayerMovement.Movement.ReadValue<Vector3>();

        rb.MovePosition(transform.position + direction * movementSpeed);
    }
}
