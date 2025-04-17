using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkObject))]
public class ThrowableObjects : NetworkBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] private float weight = 1f;
    [SerializeField] private float bounciness = 0.5f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float upwardThrowModifier = 0.5f;

    private Rigidbody rb;
    private PhysicsMaterial material;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = weight;

        material = new PhysicsMaterial
        {
            bounciness = bounciness,
            frictionCombine = PhysicsMaterialCombine.Average,
            bounceCombine = PhysicsMaterialCombine.Maximum
        };

        if (TryGetComponent(out Collider collider))
        {
            collider.material = material;
        }
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        EnablePhysicsInternal();
    }
    

    public void Throw(Vector3 direction)
    {
        rb.linearVelocity = Vector3.zero;
        Debug.Log("I Add Force Internal");
        rb.AddForce(direction.normalized * throwForce + Vector3.up * upwardThrowModifier, ForceMode.Impulse);
    }

    [ServerRpc]
    public void EnablePhysicsServerRpc()
    {
        EnablePhysicsInternal();
    }

    private void EnablePhysicsInternal()
    {
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
        }

        foreach (var col in GetComponents<Collider>())
        {
            col.enabled = true;
        }

        if (TryGetComponent<ItemPickUp>(out var pickup))
        {
            pickup.enabled = true;
        }

        transform.SetParent(null);
    }
}