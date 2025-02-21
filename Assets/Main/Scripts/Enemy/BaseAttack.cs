using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : MonoBehaviour
{
    [SerializeField] protected float damage;
    public Coroutine attackProcess;

    public LayerMask attackLayer;
    public virtual void Attack()
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
