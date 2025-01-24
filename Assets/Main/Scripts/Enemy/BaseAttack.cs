using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : MonoBehaviour
{

    protected Coroutine attackProcess;

    protected LayerMask attackLayer;
    protected virtual void Attack()
    {
        if (attackProcess != null)
            return;
        attackProcess = StartCoroutine(AttackIE());
    }
    protected virtual IEnumerator AttackIE()
    {
        return null;
    }
    protected virtual void OnDrawGizmos()
    {
    }
}
