using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerMovement : MovementBase
{
    MainController inputActions;
    private Vector3 input;
    public float FrictionAmount;
    protected override void Awake()
    {
        base.Awake();
        InputInit();
    }

    private void InputInit()
    {
        inputActions = new MainController();
        inputActions.Player.Movement.Enable();
        inputActions.Player.Movement.performed += callback => input = callback.ReadValue<Vector3>();
        inputActions.Player.Movement.canceled += callback => input = callback.ReadValue<Vector3>();
    }

    private void FixedUpdate()
    {
        Movement();
        FrictionControl();
    }

    protected override void Movement()
    {
        Vector3 direction =  transform.forward * input.z + transform.right * input.x ;
        if (direction == Vector3.zero)
            return;

        rb.AddForce(direction.normalized * movementSpeed * 10f,ForceMode.Force);

        Vector3 flatVel = new Vector3(rb.velocity.x,0,rb.velocity.z);

        if (flatVel.magnitude > movementSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void FrictionControl()
    {
        if (Mathf.Abs(input.x) < 0.01f && Mathf.Abs(input.z) < 0.01f)
        {
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            float amount = Mathf.Min(horizontalVelocity.magnitude, FrictionAmount);

            Vector3 frictionForce = horizontalVelocity.normalized * -amount;
            rb.AddForce(frictionForce, ForceMode.Impulse);
        }
    }



    private void OnDestroy()
    {
        inputActions.Disable();
    }
}
