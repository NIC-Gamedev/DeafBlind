using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowableObjects : MonoBehaviour
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

    public void Throw(Vector3 direction)
    {
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(direction.normalized * throwForce + Vector3.up * upwardThrowModifier, ForceMode.Impulse);
    }
}
