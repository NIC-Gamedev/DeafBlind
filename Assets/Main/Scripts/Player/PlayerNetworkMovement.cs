using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetworkMovement : MovementNetworkBase
{
    MainController inputActions;
    private Vector3 input;
    [Header("Walking")]
    [SerializeField] private float FrictionAmount;

    [SerializeField] private float maxStaminaTime;
    private float currentStaminaTime;

    [SerializeField] private float waitTimeBeforeStaminaRecover;
    private float currentWaitTimeBeforeStaminaRecover;

    [SerializeField] private float sneakingDevider;
    private bool isSneak;
    [Header("Jumping")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float radius;

    public bool isSprinting { get; private set; }

    private Action OnStaminaEnd;
    protected override void Awake()
    {
        base.Awake();
        currentStaminaTime = maxStaminaTime;
        currentWaitTimeBeforeStaminaRecover = waitTimeBeforeStaminaRecover;
    }

    public override void OnNetworkSpawn()
    {
        InputInit();

    }


    private void InputInit()
    {
        inputActions = InputManager.inputActions;
        inputActions.Player.Movement.Enable();
        inputActions.Player.Sprint.Enable();
        inputActions.Player.Jump.Enable();
        inputActions.Player.Sneak.Enable();
        inputActions.Player.Movement.performed += callback => input = callback.ReadValue<Vector3>();
        inputActions.Player.Movement.canceled += callback => input = callback.ReadValue<Vector3>();

        inputActions.Player.Sneak.performed += SneakPressed;


        inputActions.Player.Jump.performed += Jump;
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            return;
        }

        Movement();
        FrictionControl();

    }

    protected override void Movement()
    {
        Vector3 direction = transform.forward * input.z + transform.right * input.x;
        bool sprintInput = inputActions.Player.Sprint.IsPressed();

        if (!sprintInput)
        {
            StaminaRecover();
        }

        if (direction != Vector3.zero)
        {
            if (sprintInput)
            {
                currentWaitTimeBeforeStaminaRecover = waitTimeBeforeStaminaRecover;
                currentStaminaTime -= Time.deltaTime;

                if (currentStaminaTime < 0)
                {
                    inputActions.Player.Sprint.Disable();
                }
            }

            float speedMultiplier = sprintInput ? sprintMultiplier : 1f;
            isSprinting = speedMultiplier != 1;
            float speedDevider = isSneak ? sneakingDevider : 1f;
            rb.AddForce(direction.normalized * movementSpeed * 10f * speedMultiplier / speedDevider, ForceMode.Force);

            Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            if (flatVel.magnitude > movementSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * movementSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void StaminaRecover()
    {
        if (currentWaitTimeBeforeStaminaRecover > 0)
            currentWaitTimeBeforeStaminaRecover -= Time.deltaTime;

        if (currentStaminaTime < maxStaminaTime && currentWaitTimeBeforeStaminaRecover <= 0)
        {
            currentStaminaTime = Mathf.MoveTowards(currentStaminaTime, maxStaminaTime, Time.deltaTime);

            if (Mathf.Approximately(currentStaminaTime, maxStaminaTime))
            {
                inputActions.Player.Sprint.Enable();
            }
        }
    }

    private void Jump(InputAction.CallbackContext callback)
    {
        if (IsOnGround())
            rb.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
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

    private void SneakPressed(InputAction.CallbackContext callback)
    {
        isSneak = !isSneak;
    }

    private void OnDrawGizmos()
    {
        if (col)
            Gizmos.DrawWireSphere(col.bounds.center - new Vector3(0, col.bounds.extents.y, 0), radius);
    }

    private void OnDestroy()
    {
        inputActions.Player.Movement.Disable();
        inputActions.Player.Sprint.Disable();
        inputActions.Player.Jump.Disable();
        inputActions.Player.Sneak.Disable();
    }


    public virtual bool IsOnGround(out Collider[] collider)
    {
        Collider[] ground = Physics.OverlapSphere(col.bounds.center - new Vector3(0, col.bounds.extents.y, 0), radius, groundLayer);
        collider = ground;
        if (ground.Length > 0)
            return true;

        return false;
    }
    public virtual bool IsOnGround()
    {
        Collider[] ground = Physics.OverlapSphere(col.bounds.center - new Vector3(0, col.bounds.extents.y, 0), radius, groundLayer);
        if (ground.Length > 0)
            return true;

        return false;
    }
}
