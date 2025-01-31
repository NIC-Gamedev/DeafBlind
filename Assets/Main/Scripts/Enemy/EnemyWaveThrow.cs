using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveThrow : WaveThrow
{
    [Header("StepSound")]

    [SerializeField] protected AudioClip playerMoveSound;
    public float stepTime;
    protected float curentStepTime;
    [Range(0, 1)][SerializeField] protected float stepVol = 1;
    [Range(0, 1)][SerializeField] protected float stepPitch = 1;
    [Range(0, 1)][SerializeField] protected float stepPitchRandomMin = 0;
    [Range(0, 1)][SerializeField] protected float stepPitchRandomMax = 0;

    [SerializeField] private float soundMin = 0;
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
        if (Mathf.Abs(rb.velocity.x) > 4 || Mathf.Abs(rb.velocity.z) > 4)
        {
            curentStepTime = stepTime;
            audioManager.PlaySoundEffect(playerMoveSound, volume: 0.5f, minDistance: soundMin, maxDistance: soundMax, ColideObject: collision, soundObject: gameObject);
        }
        else
        {
            curentStepTime -= Time.deltaTime;
        }
    }
}
