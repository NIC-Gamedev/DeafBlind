using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class EnemyWaveThrow : WaveThrow
{
    [Header("StepSound")]

    [SerializeField] protected EventReference footstepSound;
    public float stepTime;
    protected float curentStepTime;
    [Range(0, 1)][SerializeField] protected float stepVol = 1;
    [Range(0, 1)][SerializeField] protected float stepPitch = 1;
    
    [SerializeField] private float soundMax = 10;

    private Vector3 direction;

    private Vector3 colliderBottom => col.bounds.center - new Vector3(0, col.bounds.extents.y, 0);
    private Collider _col;
    private Collider col
    {
        get
        {
            if (_col == null)
            {
                _col = GetComponent<Collider>();
            }
            return _col;
        }
    }

    protected override void Start()
    {
        base.Start();
        curentStepTime = stepTime;
    }
    protected void OnCollisionStay(Collision collision)
    {
        if (curentStepTime < 0 &&( Mathf.Abs(rb.linearVelocity.x) > 1 || Mathf.Abs(rb.linearVelocity.z) > 1))
        {
            curentStepTime = stepTime;
            audioManager.InstanceByPos(footstepSound,collision.contacts[0].point,stepVol,stepPitch);
        }
        else
        {
            curentStepTime -= Time.deltaTime;
        }
    }
}
