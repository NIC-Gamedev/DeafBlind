using System.Collections;
using System.Reflection;
using UnityEngine;

public class WaveThrow : MonoBehaviour
{
    protected Rigidbody rb;
    [SerializeField] protected AudioClip colideSound;
    protected AudioManager audioManager => AudioManager.instance;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        audioManager.PlaySoundEffect(colideSound, volume:1, ColideObject: collision);
    }
}
