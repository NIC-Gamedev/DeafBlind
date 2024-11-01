using UnityEngine;
using UnityEngine.InputSystem;
public abstract class MovementBase : MonoBehaviour
{
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float sprintMultiplier;
    [SerializeField] protected float jumpForce;

    public Rigidbody rb { get; private set; }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
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