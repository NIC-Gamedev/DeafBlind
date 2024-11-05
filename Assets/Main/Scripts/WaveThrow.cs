using UnityEngine;

public class WaveThrow : MonoBehaviour
{
    [SerializeField] private ParticleSystem waveParticle;
    private Rigidbody rb;

    private void OnCollisionEnter(Collision collision)
    {
        if(rb.velocity != Vector3.zero)
        {
            ParticleSystem instance = Instantiate(waveParticle,transform.position,Quaternion.identity);
        }
    }
}
