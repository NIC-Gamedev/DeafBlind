using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScanerThrow : WaveThrow
{
    public float stepTime;
    protected float curentStepTime;

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
            ThrowWave(collision,0.4f, (Mathf.Abs(vel.x) + Mathf.Abs(vel.y) + Mathf.Abs(vel.z)) / 5,2);
            curentStepTime = stepTime;
        }
    }
}
