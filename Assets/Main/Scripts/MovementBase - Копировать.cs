using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;
public abstract class MovementNetworkBase : NetworkBehaviour
{
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float sprintMultiplier;
    [SerializeField] protected float jumpForce;

    public Rigidbody rb { get; private set; }

    protected Collider col;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    protected virtual void Movement()
    {
    }
    protected virtual void Movement(InputAction.CallbackContext callback)
    {
    }
    protected virtual void Movement(Vector3 direction)
    {
    }
}