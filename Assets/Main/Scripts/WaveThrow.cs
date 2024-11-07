using System.Reflection;
using UnityEngine;

public class WaveThrow : MonoBehaviour
{
    [SerializeField] protected ParticleSystem waveParticle;
    protected Rigidbody rb;
    public Vector3 vel;

    protected virtual void LateUpdate()
    {
        vel = rb.velocity;
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        ParticleSystem instance = Instantiate(waveParticle, collision.contacts[0].point, Quaternion.identity);
        var main = instance.main;
        main.startLifetime = (Mathf.Abs(vel.x) + Mathf.Abs(vel.y) + Mathf.Abs(vel.z));
        main.startSize = (Mathf.Abs(vel.x) + Mathf.Abs(vel.y) + Mathf.Abs(vel.z)) * 5; 
    }
}
