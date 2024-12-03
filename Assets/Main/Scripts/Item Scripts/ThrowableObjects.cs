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
    private PhysicMaterial material;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = weight;

        material = new PhysicMaterial
        {
            bounciness = bounciness,
            frictionCombine = PhysicMaterialCombine.Average,
            bounceCombine = PhysicMaterialCombine.Maximum
        };

        if (TryGetComponent(out Collider collider))
        {
            collider.material = material;
        }
    }

    public void Throw(Vector3 direction)
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(direction.normalized * throwForce + Vector3.up * upwardThrowModifier, ForceMode.Impulse);
    }
}
