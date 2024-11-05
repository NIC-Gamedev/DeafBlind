using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MovementBase
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
    [SerializeField] private Transform groundCheacker;
    [SerializeField] private float radius;

    private Action OnStaminaEnd;
    protected override void Awake()
    {
        base.Awake();
        InputInit();
        currentStaminaTime = maxStaminaTime;
        currentWaitTimeBeforeStaminaRecover = waitTimeBeforeStaminaRecover;
    }


    private void InputInit()
    {
        inputActions = new MainController();
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
        Movement();
        FrictionControl();
    }

    protected override void Movement()
    {
        Vector3 direction = transform.forward * input.z + transform.right * input.x;
        bool isSprinting = inputActions.Player.Sprint.IsPressed();

        if (!isSprinting)
        {
            StaminaRecover();
        }

        if (direction != Vector3.zero)
        {
            if (isSprinting)
            {
                currentWaitTimeBeforeStaminaRecover = waitTimeBeforeStaminaRecover;
                currentStaminaTime -= Time.deltaTime;

                if (currentStaminaTime < 0)
                {
                    inputActions.Player.Sprint.Disable();
                }
            }

            float speedMultiplier = isSprinting ? sprintMultiplier : 1f;
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
        if(currentWaitTimeBeforeStaminaRecover > 0) 
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
        Collider[] ground = Physics.OverlapSphere(groundCheacker.position, radius, groundLayer);
        if (ground.Length > 0)
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
        if(groundCheacker) 
            Gizmos.DrawWireSphere(groundCheacker.position, radius);
    }

    private void OnDestroy()
    {
        inputActions.Player.Movement.Disable();
        inputActions.Player.Sprint.Disable();
        inputActions.Player.Jump.Disable();
        inputActions.Player.Sneak.Disable();
    }
}
