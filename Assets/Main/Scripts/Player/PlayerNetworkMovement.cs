using UnityEngine;
using UnityEngine.InputSystem;
using System;
using FishNet.Object;
using static UnityEngine.InputManagerEntry;
using FishNet.Object.Synchronizing;
using System.Collections;

public class PlayerNetworkMovement : MovementNetworkBase
{
    public Vector3 input { get; private set; }
    [Header("Walking")]
    [SerializeField] private float FrictionAmount;

    [SerializeField] private float maxStaminaTime;
    private float currentStaminaTime;

    [SerializeField] private float waitTimeBeforeStaminaRecover;
    private float currentWaitTimeBeforeStaminaRecover;

    [SerializeField] private float sneakingDevider;
    public bool isSneak { get; set; }
    [Header("Jumping")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float radius;

    public bool isSprinting { get; private set; }

    private Action OnStaminaEnd;

    public MainController inputActions;
    public bool isGrounded { private set; get; }

    public Vector3 ColliderCenterOnSneak;
    public float heightOnSneak;

    private float heightTemp;



    private bool isSneakReceived = false;
    protected override void Awake()
    {
        base.Awake();
        currentStaminaTime = maxStaminaTime;
        currentWaitTimeBeforeStaminaRecover = waitTimeBeforeStaminaRecover;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if(base.IsOwner)
        {
            InputInit();
        }
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

        heightTemp = (col as CapsuleCollider).height;
        inputActions.Player.Jump.performed += Jump;
    }

    private void FixedUpdate()
    {
        if (base.IsOwner)
        {
            Movement();

            bool onGround = IsOnGround();
            if (isGrounded != onGround)
            {
                isGrounded = onGround;
                SendServerGroundParam(isGrounded);
            }
            FrictionControl();
        }
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

            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            if (flatVel.magnitude > movementSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * movementSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
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
        if (isGrounded)
            rb.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
    }

    void FrictionControl()
    {
        if (Mathf.Abs(input.x) < 0.01f && Mathf.Abs(input.z) < 0.01f)
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            float amount = Mathf.Min(horizontalVelocity.magnitude, FrictionAmount);

            Vector3 frictionForce = horizontalVelocity.normalized * -amount;
            rb.AddForce(frictionForce, ForceMode.Impulse);
        }
    }
    private void SneakPressed(InputAction.CallbackContext callback)
    {
        if (base.IsOwner)
        {
            if (gameObject.name == "Blind")
                Debug.Log("It's Blind");

            var capsuleCol = (col as CapsuleCollider);
            isSneak = !isSneak;
            SendServerSneakParam(isSneak);
            if (isSneak)
            {
                capsuleCol.center = ColliderCenterOnSneak;
                capsuleCol.height = heightOnSneak;
            }
            else
            {
                capsuleCol.height = heightTemp;
                capsuleCol.center = Vector3.zero;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (col)
            Gizmos.DrawWireSphere(col.bounds.center - new Vector3(0, col.bounds.extents.y, 0), radius);
    }

    private void OnDestroy()
    {
        inputActions.Player.Movement.performed -= callback => input = callback.ReadValue<Vector3>();
        inputActions.Player.Movement.canceled -= callback => input = callback.ReadValue<Vector3>();

        inputActions.Player.Sneak.performed -= SneakPressed;

        inputActions.Player.Jump.performed -= Jump;
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
    [ServerRpc]
    public void SendServerGroundParam(bool onGround)
    {
        Debug.Log($"{gameObject} is Grounded");
        isGrounded = onGround;
    }
    [ServerRpc]
    public void SendServerSneakParam(bool sneakParam)
    {
        isSneakReceived = sneakParam;
        Debug.Log($"Sneack is {isSneak}");
    }
}
