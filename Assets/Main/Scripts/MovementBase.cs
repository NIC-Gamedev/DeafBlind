using UnityEngine;
public abstract class MovementBase : MonoBehaviour
{
    protected float movementSpeed;
    protected float sprintMultiplier;
    protected float jumpForce;

    public Rigidbody rb { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Movement()
    {
        return;
    }
}