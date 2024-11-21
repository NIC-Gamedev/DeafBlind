using System.Collections;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class WaveThrow : MonoBehaviour
{
    protected Rigidbody rb;
    [Header("ColideSound")]
    [SerializeField] protected AudioClip colideSound;

    [Range(0,1)] [SerializeField] protected float volume = 1;
    [Range(0,1)] [SerializeField] protected float pitch = 1;
    [Range(0,1)] [SerializeField] protected float pitchRandomMin = 0;
    [Range(0,1)] [SerializeField] protected float pitchRandomMax = 0;

    [SerializeField] protected float minDistance;
    [SerializeField] protected float maxDistance;
    protected AudioManager audioManager => AudioManager.instance;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (pitchRandomMin > 0 || pitchRandomMax > 0)
        {
            pitch = Random.Range(pitchRandomMin, pitchRandomMax);
        }
        audioManager.PlaySoundEffect(colideSound, volume: volume, minDistance:minDistance,maxDistance:maxDistance,ColideObject: collision,pitch: pitch);
    }
}
