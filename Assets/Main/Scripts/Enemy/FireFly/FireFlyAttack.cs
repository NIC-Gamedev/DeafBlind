using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlyAttack : EnemyDistanceAttack
{
    [SerializeField] protected ParticleSystem fireTrow;


    protected override IEnumerator AttackIE()
    {
        yield return new WaitForSeconds(0.2f);
        fireTrow.Emit(1);
    }


    private void Update()
    {
        
    }
}
