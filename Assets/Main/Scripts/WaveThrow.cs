using System.Collections;
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
        ThrowWave(collision, (Mathf.Abs(vel.x) + Mathf.Abs(vel.y) + Mathf.Abs(vel.z)), (Mathf.Abs(vel.x) + Mathf.Abs(vel.y) + Mathf.Abs(vel.z)) * 5,5);
    }

    protected void ThrowWave(Collision collision,float startLifeTime = 1, float startSize = 1,int emitCount = 1)
    {
        ParticleSystem instance = Instantiate(waveParticle, collision.contacts[0].point, Quaternion.identity);
        var main = instance.main;
        main.startLifetime = startLifeTime;
        main.startSize = startSize;
        StartCoroutine(EmitWave(instance, emitCount));
    }
    protected IEnumerator EmitWave(ParticleSystem particle,int count)
    {
        while (count > 0)
        {
            if (particle)
            {
                count--;
                particle.Emit(1);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
