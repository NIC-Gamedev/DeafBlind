using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScanerThrow : WaveThrow
{
    public float stepTime;
    protected float curentStepTime;
    [SerializeField] protected AudioClip playerMoveSound;
    protected override void Start()
    {
        base.Start();
        curentStepTime = stepTime;
    }
    protected void OnCollisionStay(Collision collision)
    {
        curentStepTime -= Time.deltaTime;  
        if (rb.velocity != Vector3.zero && curentStepTime < 0)
        {
            curentStepTime = stepTime;
            audioManager.PlaySoundEffect(playerMoveSound, volume: 0.1f, ColideObject: collision);
        }
    }
}
